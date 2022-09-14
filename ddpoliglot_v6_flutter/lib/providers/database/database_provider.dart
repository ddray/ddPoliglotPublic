import 'package:ddpoliglot_v6_flutter/models/user_settings_state_data.dart';
import 'package:ddpoliglot_v6_flutter/models/article_by_param_data.dart';
import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:ddpoliglot_v6_flutter/services/article_by_param_service.dart';
import 'package:ddpoliglot_v6_flutter/services/database_service.dart';
import 'package:ddpoliglot_v6_flutter/services/word_service.dart';
import 'package:ddpoliglot_v6_flutter/utils/http_utils.dart';
import 'package:ddpoliglot_v6_flutter/widgets/loading_generate_lesson_widget.dart';
import 'package:flutter/material.dart';
import 'package:collection/collection.dart';

import '../auth/auth_state.dart';
import 'database_state.dart';

class DatabaseProvider with ChangeNotifier {
  DatabaseState _state = DatabaseState.initial();
  DatabaseState get state => _state;

  final AuthState authState;
  final UserSettingsState? userSettingsStateData;

  String _databaseName = '';
  LoadingData loadingData = LoadingData();

  // settings candidates
  final int _getRepWordsFromLastLessons = 3;
  // final int _getMaxRepWordsAdditional = 10;

  DatabaseProvider({
    required this.authState,
    required this.userSettingsStateData,
    required DatabaseState newState,
  }) {
    if (authState.isAuth && userSettingsStateData != null) {
      _databaseName =
          'ddPoliglotV6_${authState.userId}_${userSettingsStateData!.learnLanguage!.languageID}_${userSettingsStateData!.nativeLanguage!.languageID}.db';
    }

    _state = newState;
  }

  Future<void> tryToRestoreFromStore(Function(LoadingData) showState) async {
    var wordsTmp = state.dictWords;
    _state = _state.copyWith(database: DatabaseService.openDb(_databaseName));
    final db = await _state.database;
    final List<Map<String, dynamic>> versions = (await db!.query('vversions'));
    final dictVersion = versions.first['value'];
    final List<Map<String, dynamic>> schemaVersions =
        (await db.query('schema_versions'));
    final schemaVersion = schemaVersions.first['value'];

    final isOnLine = await HttpUtils.isOnLine();
    DictionaryResponse? dictionaryResponse;
    SchemaResponse? schemaResponse;

    if (isOnLine) {
      loadingData = loadingData = loadingData.copyWith(
          message1: 'Загружаем словарь',
          message2: 'Идет загрузка...',
          progress1: 30,
          progress2: 10);

      showState(loadingData);

      dictionaryResponse = await WordService.getDictionary(dictVersion);

      loadingData =
          loadingData = loadingData.copyWith(progress1: 40, progress2: 80);
      showState(loadingData);

      schemaResponse = await ArticleByParamService.getSchemas(schemaVersion);

      loadingData = loadingData = loadingData.copyWith(
          message1: 'Загружаем Schema', progress1: 60, progress2: 60);
      showState(loadingData);
    } else {}

    loadingData =
        loadingData = loadingData.copyWith(progress1: 70, progress2: 10);
    showState(loadingData);

    if (isOnLine && dictionaryResponse!.version != dictVersion) {
      // new dictionary version

      loadingData = loadingData = loadingData.copyWith(
          message1: 'Загружаем данные новой версии',
          progress1: 80,
          progress2: 40);
      showState(loadingData);

      // dictWords
      wordsTmp = dictionaryResponse.dictWords;
      await db.delete('dictWords');
      var batch = db.batch();
      for (DictWord dictWord in wordsTmp) {
        batch.insert('dictWords', dictWord.toJson());
      }
      await batch.commit(noResult: true);

      // userLessons
      _state = _state.copyWith(userLessons: dictionaryResponse.userLessons);
      await db.delete('userLessons');
      batch = db.batch();
      for (UserLesson userLesson in _state.userLessons ?? []) {
        batch.insert('userLessons', userLesson.toDbJson());
      }
      await batch.commit(noResult: true);

      // vversions
      await db
          .update('vversions', {'id': 1, 'value': dictionaryResponse.version});
    } else {
      loadingData = loadingData = loadingData.copyWith(
          message1: 'Загружаем данные 2', progress1: 20, progress2: 30);
      showState(loadingData);

      // fresh data is already in db
      wordsTmp = (await db.query('dictWords', orderBy: 'rate')).map((x) {
        return DictWord.fromJson(x);
      }).toList();

      _state = _state.copyWith(
          userLessons: (await db.query('userLessons', orderBy: 'num')).map((x) {
        return UserLesson.fromDbJson(x);
      }).toList());

      loadingData =
          loadingData = loadingData.copyWith(progress1: 40, progress2: 60);
      showState(loadingData);
    }

    // fill lessons with words
    for (UserLesson userLesson in (state.userLessons ?? [])) {
      for (var userLessonWord in userLesson.userLessonWords) {
        userLessonWord.dictWord =
            wordsTmp.firstWhereOrNull((x) => x.wordID == userLessonWord.wordID);
      }

      // remove words from lessons which does not exists, may be were deleted
      userLesson.userLessonWords =
          userLesson.userLessonWords.where((x) => x.dictWord != null).toList();
    }

    loadingData =
        loadingData = loadingData.copyWith(progress1: 40, progress2: 60);
    showState(loadingData);

    // check for new lessons schema versions

    if (isOnLine && schemaResponse!.version != schemaVersion) {
      // new schema version
      _state = _state.copyWith(schemas: schemaResponse.schemas);
      await db.delete('schemaLessonGen');
      var batch = db.batch();
      for (var i = 0; i < schemaResponse.schemas.length; i++) {
        batch.insert('schemaLessonGen', schemaResponse.schemas[i].toDb(i));
      }

      await batch.commit(noResult: true);

      // sversions
      await db.update(
          'schema_versions', {'id': 1, 'value': schemaResponse.version});
    } else {
      // fresh data is already in db
      _state = _state.copyWith(
          schemas: (await db.query('schemaLessonGen')).map((x) {
        return ArticleByParamData.fromDb(x);
      }).toList());
    }

    debugPrint('DATABASE SAVE PREFS with SUCCESS');

    loadingData = loadingData = loadingData.copyWith(
        message1: 'Загружаем',
        message2: 'Идет загрузка ...',
        progress1: 90,
        progress2: 95);
    showState(loadingData);

    // notify that database is updated
    _state = _state.copyWith(dictWords: wordsTmp);
    notifyListeners();
  }

  List<DictWord> getLessonCandidates() {
    for (var word in state.dictWords!) {
      word.selected = false;
    }

    return state.dictWords!.where((x) => x.grade == 0).toList();
  }

  List<DictWord> getLessonRepetitionCandidates() {
    // get words for repetitions from last 3 lessons
    List<DictWord> result = [];
    state.userLessons!.sort((a, b) => b.num.compareTo(a.num));
    var lastLessons = state.userLessons!.take(_getRepWordsFromLastLessons);
    for (UserLesson userLesson in lastLessons) {
      for (var userLessonWord in userLesson.userLessonWords) {
        if (userLessonWord.dictWord!.grade > 0 &&
            userLessonWord.dictWord!.grade < 5 &&
            userLessonWord.wordType == 0) {
          result.add(userLessonWord.dictWord!);
        }
      }
    }

    return result;
  }

  List<DictWord> getTestWords() {
    List<DictWord> result = [];
    int step = 0;
    for (var dictWord in state.dictWords!) {
      if (++step == 4) {
        step = 0;
        result.add(dictWord);
      }
    }

    return result;
  }

  UserLesson getLessonByNum(int runLessonNum) {
    return state.userLessons!.firstWhere((x) => x.num == runLessonNum);
  }
}
