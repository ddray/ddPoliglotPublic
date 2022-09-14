import 'dart:convert';
import 'dart:io' as io;

import 'package:audioplayers/audioplayers.dart';
import 'package:ddpoliglot_v6_flutter/models/aPhWr.dart';
import 'package:ddpoliglot_v6_flutter/models/user_settings_state_data.dart';
import 'package:ddpoliglot_v6_flutter/models/article_by_param_data.dart';
import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:ddpoliglot_v6_flutter/models/play_list.dart';
import 'package:ddpoliglot_v6_flutter/models/play_list_item.dart';
import 'package:ddpoliglot_v6_flutter/models/user_lesson_data.dart';
import 'package:ddpoliglot_v6_flutter/providers/playlist_provider.dart/play_list_state.dart';
import 'package:ddpoliglot_v6_flutter/services/database_service.dart';
import 'package:ddpoliglot_v6_flutter/services/user_lesson_service.dart';
import 'package:ddpoliglot_v6_flutter/utils/http_utils.dart';
import 'package:flutter/material.dart';
import 'package:path_provider/path_provider.dart' as syspath;
import 'package:shared_preferences/shared_preferences.dart';
import 'dart:async';
import 'package:collection/collection.dart';

import '../../models/player_classes.dart';
import '../../models/data/Word.dart';
import '../../utils/play_list_utils.dart';
import '../auth/auth_state.dart';
import '../database/database_state.dart';

class PlayListProvider with ChangeNotifier {
  PlayListState _state = PlayListState.initial();
  PlayListState get state => _state;

  final AuthState authState;
  final UserSettingsState? userSettingsStateData;
  final DatabaseState? databaseState;

  bool _loading = false;

  ArticleByParamData? _articleByParamData;
  AudioSPlayerState _playerState = AudioSPlayerState.stoped;

  // ignore: prefer_final_fields
  AudioPlayer _player = AudioPlayer();
  Timer? _timer;
  String _keyPref = '';

  PlayListProvider({
    required this.authState,
    required this.userSettingsStateData,
    required this.databaseState,
    required PlayListState newState,
  }) {
    _state = newState;
    if (!authState.isAuth ||
        userSettingsStateData == null ||
        (databaseState?.dictWords ?? []).isEmpty ||
        (databaseState?.schemas ?? []).isEmpty) {
      _state = _state.copyWith(userLessonData: null);
    } else {
      _keyPref = 'playlistDataV1_user_${authState.userId}';

      if (state.userLessonData == null) {
        _state = _state.copyWith(
            preparationStep: PreparationStep.showUserData.index);
        tryToRestoreFromStore();
      }

      debugPrint(
          '---PlayListProvider ${authState.token}, ${authState.userId}, $playLists');

      _player.onPlayerComplete.listen((event) async {
        _timer =
            Timer(Duration(milliseconds: (currentItem!.pause * 1000).toInt()),
                () async {
          if (_playerState == AudioSPlayerState.plaing) {
            await goNext();
          }
        });
      });
    }
  }

  setPreparationStep(int step) {
    _state = _state.copyWith(preparationStep: step);
    debugPrint('PLAYLIST NOTIFY1: step: $step');
    debugPrint('********* +++++ PlayListProvider notifyListeners1');
    notifyListeners();
  }

  bool get finished {
    return state.userLessonData?.finished ?? false;
  }

  List<PlayList>? get playLists {
    return state.userLessonData?.playLists;
  }

  List<Word> get words {
    return state.userLessonData!.getWords();
  }

  bool get isloading {
    return _loading;
  }

  AudioSPlayerState get playerState {
    return _playerState;
  }

  Future<void> tryCurrentToPlay() async {
    if (_playerState == AudioSPlayerState.plaing) {
      await _player.stop();
      if (_timer != null) {
        _timer?.cancel();
      }

      if (state.userLessonData?.startPlay == null &&
          state.userLessonData != null) {
        state.userLessonData!.startPlay = DateTime.now();
      }

      final appDir = await syspath.getApplicationDocumentsDirectory();
      var url = '${appDir.path}/${currentItem!.textSpeechFileName}';
      await _player.play(DeviceFileSource(url));
    }
  }

  Future<void> playerPlay() async {
    _playerState = AudioSPlayerState.plaing;
    await processChanges();
  }

  Future<void> playerStop({notify = true}) async {
    await _player.stop();
    if (_timer != null) {
      _timer?.cancel();
    }

    if (state.userLessonData?.startPlay != null) {
      var date = state.userLessonData!.startPlay!;
      var curdate = DateTime.now();
      var dif = curdate.difference(date).inSeconds;
      state.userLessonData!.userLesson!.learnSeconds += dif;
      state.userLessonData!.startPlay = null;
    }

    await save(notify: false);

    _playerState = AudioSPlayerState.stoped;
    if (notify) {
      await processChanges();
    }
  }

  Future<void> setCurrentPlayListIndex(int index) async {
    if ((state.userLessonData?.playLists ?? []).isEmpty ||
        (state.userLessonData?.playLists ?? []).length < (index + 1)) {
      return;
    }

    await playerStop();

    state.userLessonData!.currentPlayListIndex = index;
    state.userLessonData!.currentItemNum = 0;

    await processChanges();
  }

  Future<void> goToStart() async {
    await setCurrentPlayListIndex(0);
    state.userLessonData!.finished = false;
    await save(notify: false);
    await processChanges();
  }

  int get currentPlayListIndex {
    return state.userLessonData?.currentPlayListIndex ?? 0;
  }

  PlayListItem? get currentItem {
    return state.userLessonData?.getCurrentItem();
  }

  double get itemsCount {
    return state.userLessonData?.getItemsCount() ?? 0;
  }

  double get currentItemIndex {
    return (state.userLessonData?.currentItemNum ?? 0).toDouble();
  }

  Future<void> goTo(double index) async {
    if ((state.userLessonData?.playLists ?? []).isEmpty) return;
    await state.userLessonData!.goTo(index);
    await processChanges();
  }

  Future<void> goFirst() async {
    if ((state.userLessonData?.playLists ?? []).isEmpty) return;
    state.userLessonData!.currentItemNum = 0;
    await processChanges();
  }

  Future<void> goNext() async {
    if ((state.userLessonData?.playLists ?? []).isEmpty) return;

    if (++state.userLessonData!.currentItemNum >=
        state
            .userLessonData!
            .playLists![state.userLessonData!.currentPlayListIndex]
            .items
            .length) {
      if (++state.userLessonData!.currentPlayListIndex >=
          state.userLessonData!.playLists!.length) {
        await finishLesson();
      } else {
        state.userLessonData!.currentItemNum = 0;
      }
    }

    await processChanges();
  }

  Future<void> goPrev() async {
    if ((state.userLessonData?.playLists ?? []).isEmpty) return;

    if (--state.userLessonData!.currentItemNum < 0) {
      if (state
          .userLessonData!
          .playLists![state.userLessonData!.currentPlayListIndex]
          .items
          .isNotEmpty) {
        state.userLessonData!.currentItemNum = state
                .userLessonData!
                .playLists![state.userLessonData!.currentPlayListIndex]
                .items
                .length -
            1;
      } else {
        state.userLessonData!.currentItemNum = 0;
      }
    }

    await processChanges();
  }

  Future<void> finishLessonAndPlayOther({int lessonNum = 0}) async {
    if (state.userLessonData?.userLesson != null) {
      state.userLessonData!.finished = true;
      await UserLessonService.update(state.userLessonData!.userLesson!);
      await save(notify: false);
    }
    _state = _state.copyWith(userLessonData: UserLessonData());
    state.userLessonData!.runLessonNum = lessonNum;
    _state =
        _state.copyWith(preparationStep: PreparationStep.configureLesson.index);
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_keyPref);
    _loading = false;
    _playerState = AudioSPlayerState.stoped;
    await processChanges();
  }

  Future<void> finishLesson() async {
    // await playerStop();
    // userLessonData!.finished = true;
    // await UserLessonService.update(userLessonData!.userLesson!);
    // await save(notify: false);
    // await processChanges();

    await playerStop(notify: false);
    state.userLessonData!.finished = true;
    await UserLessonService.update(state.userLessonData!.userLesson!);
    // await save(notify: false);
    await clearLesson();
  }

  Future<void> cleanProperties() async {
    await clearLesson();
    final prefs = await SharedPreferences.getInstance();
    if (prefs.containsKey('userData')) {
      prefs.clear();
    }

    await processChanges();
  }

  Future<void> clearLesson(
      {PreparationStep step = PreparationStep.showUserData,
      int lessonNum = 0}) async {
    await playerStop();
    _state = _state.copyWith(
        userLessonData: UserLessonData(), preparationStep: step.index);
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_keyPref);
    _loading = false;
    _playerState = AudioSPlayerState.stoped;
    await processChanges();
  }

  Future<void> processChanges() async {
    await tryCurrentToPlay();

    debugPrint('PLAYLIST NOTIFY2');
    debugPrint('********* +++++ PlayListProvider notifyListeners2');
    notifyListeners();
    // if (!_isDisposed) {
    //   notifyListeners();
    // }
  }

  void setLoading(bool value) {
    _loading = value;

    // if (!_isDisposed) {
    //   notifyListeners();
    // }
    debugPrint('PLAYLIST NOTIFY3');
    debugPrint('********* +++++ PlayListProvider notifyListeners3');
    notifyListeners();
  }

  Future<void> setWordsAndGeneratePlayLists(
      List<DictWord> selectedWords,
      List<DictWord>? wordsForRepetition,
      int lessonNum,
      int lessonType,
      bool isNew,
      Function(bool value,
              {String? label, String? msg, int? procent1, int? procent2})
          isLoading) async {
    debugPrint('setWordsAndGeneratePlayLists');

    final isOnLine = await HttpUtils.isOnLine();

    setLoading(true);
    debugPrint('setWordsAndGeneratePlayLists 2');
    await playerStop();
    debugPrint('setWordsAndGeneratePlayLists 3');
    isLoading(true,
        label: "Generating new Lesson. It can take some time.",
        msg: "Start Create new lesson",
        procent1: 0,
        procent2: 0);

    _state = _state.copyWith(userLessonData: UserLessonData());
    debugPrint('setWordsAndGeneratePlayLists 4');
    await createNextUserLesson(
        selectedWords, wordsForRepetition, lessonNum, lessonType);
    debugPrint('setWordsAndGeneratePlayLists 5');
    isLoading(true,
        msg: "Loading words and phrases data", procent1: 10, procent2: 20);
    debugPrint('setWordsAndGeneratePlayLists 6');
    isLoading(true, msg: "Fetch Article By Param", procent1: 20, procent2: 30);
    debugPrint('setWordsAndGeneratePlayLists 7');
    await fetchArticleByParamAndDataWithAudioData(lessonType, isNew, isOnLine);
    debugPrint('setWordsAndGeneratePlayLists 8');
    isLoading(true,
        msg: "Start generating playlists", procent1: 50, procent2: 80);
    debugPrint('setWordsAndGeneratePlayLists 9');
    await generatePlayLists(lessonType, isLoading, isOnLine);
    debugPrint('setWordsAndGeneratePlayLists 10');

    save();

    setLoading(false);
    debugPrint("fetchAndSet: downloaded");
  }

  Future<void> createNextUserLesson(List<DictWord> selectedWords,
      List<DictWord>? wordsForRepetition, int lessonNum, int lessonType) async {
    var userLesson = await DatabaseService.userLessonById(
        databaseState!.database, lessonNum);
    var isNew = true;
    List<Word> wordList;
    List<Word> wordListForRepetition;
    Map<String, dynamic> data;
    if (userLesson == null) {
      data = await UserLessonService.createNext(
          selectedWords, wordsForRepetition!, lessonType, lessonNum);
      userLesson = data['userLesson'] as UserLesson?;
      wordList = data['words'] as List<Word>;
      wordListForRepetition = data['repetition'] as List<Word>;
    } else if (userLesson.jsonData != null) {
      isNew = false;
      final decodeBase64Json = base64.decode(userLesson.jsonData ?? '');
      final decodegZipJson = io.gzip.decode(decodeBase64Json);
      final originalJson = utf8.decode(decodegZipJson);
      data = json.decode(originalJson);

      wordList = (data['words'] as List<dynamic>)
          .map((e) => Word.fromJson(e as Map<String, dynamic>))
          .toList();
      wordListForRepetition = (data['repetition'] as List<dynamic>)
          .map((e) => Word.fromJson(e as Map<String, dynamic>))
          .toList();
    } else {
      isNew = false;
      // there is no stored data
      data = await UserLessonService.createNext(
          selectedWords, wordsForRepetition!, lessonType, lessonNum);
      userLesson = data['userLesson'] as UserLesson?;
      wordList = data['words'] as List<Word>;
      wordListForRepetition = data['repetition'] as List<Word>;
    }

    if (userLesson?.userLessonWords != null) {
      for (var userLessonWord in userLesson!.userLessonWords) {
        if (userLessonWord.wordType == 0) {
          // main
          userLessonWord.word = wordList
              .firstWhereOrNull((word) => word.wordID == userLessonWord.wordID);
        } else {
          // repetition
          userLessonWord.word = wordListForRepetition
              .firstWhereOrNull((word) => word.wordID == userLessonWord.wordID);
        }

        if (userLessonWord.word != null) {
          // save to lDb and lists
          var dictWord = databaseState!.dictWords!
              .firstWhereOrNull((x) => x.wordID == userLessonWord.word!.wordID);

          if (dictWord != null && (isNew || userLesson.jsonData == null)) {
            dictWord.setWordUserData(userLessonWord.word!);
            await DatabaseService.dictWordUpdate(
                databaseState!.database, dictWord);
          }

          userLessonWord.dictWord = dictWord;
        }
      }

      // remove words which does not exists now, may be were deleted
      userLesson.userLessonWords = userLesson.userLessonWords
          .where((x) => x.word != null && x.dictWord != null)
          .toList();

      if (isNew || userLesson.jsonData == null) {
        // save to lDb and lists

        // compress lesson word data
        var jsonData = json.encode(data);
        final enCodedJson = utf8.encode(jsonData);
        final gZipJson = io.gzip.encode(enCodedJson);
        final base64Json = base64.encode(gZipJson);
        userLesson.jsonData = base64Json;

        if (isNew) {
          await DatabaseService.userLessonInsert(
              databaseState!.database, userLesson, databaseState!.userLessons);
        } else {
          await DatabaseService.userLessonUpdate(
              databaseState!.database, userLesson, databaseState!.userLessons);
        }
      }

      state.userLessonData!.userLesson = userLesson;
    }

    await processChanges();
  }

  Future<void> fetchArticleByParamAndDataWithAudioData(
      int lessonType, bool isNew, bool isOnLine) async {
    _articleByParamData =
        databaseState!.schemas![lessonType].cloneWithCloneDictorPhrases();
    final appDir = await syspath.getApplicationDocumentsDirectory();

    for (var mixParam in _articleByParamData!.mixParamsList) {
      mixParam.firstDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.firstDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.firstBeforeDialogDictorPhrasesWithAudioW =
          await filterExistsOnly(
              mixParam.firstBeforeDialogDictorPhrasesWithAudioW,
              appDir,
              isOnLine);
      mixParam.beforeByOrderMixDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.beforeByOrderMixDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.insideByOrderMixDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.insideByOrderMixDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudioW =
          await filterExistsOnly(
              mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudioW,
              appDir,
              isOnLine);
      mixParam.insideBaseWordsDirMixDictorPhrasesWithAudioW =
          await filterExistsOnly(
              mixParam.insideBaseWordsDirMixDictorPhrasesWithAudioW,
              appDir,
              isOnLine);
      mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudioW =
          await filterExistsOnly(
              mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudioW,
              appDir,
              isOnLine);
      mixParam.insideBaseWordsRevMixDictorPhrasesWithAudioW =
          await filterExistsOnly(
              mixParam.insideBaseWordsRevMixDictorPhrasesWithAudioW,
              appDir,
              isOnLine);
      mixParam.beforeAllDirMixDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.beforeAllDirMixDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.insideAllDirMixDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.insideAllDirMixDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.beforeAllRevMixDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.beforeAllRevMixDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.insideAllRevMixDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.insideAllRevMixDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.beforeFinishDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.beforeFinishDictorPhrasesWithAudioW, appDir, isOnLine);
      mixParam.finishDictorPhrasesWithAudioW = await filterExistsOnly(
          mixParam.finishDictorPhrasesWithAudioW, appDir, isOnLine);
    }
  }

  Future<List<List<List<APhWr>>>> filterExistsOnly(
      List<List<List<APhWr>>> source,
      io.Directory appDir,
      bool isOnLine) async {
    List<List<List<APhWr>>> res = [];
    for (var l1 in source) {
      List<List<APhWr>> r1 = [];
      res.add(r1);
      for (var l2 in l1) {
        List<APhWr> r2 = [];
        r1.add(r2);
        for (var item in l2) {
          bool add = isOnLine
              ? true
              : await io.File('${appDir.path}/${item.tfName}').exists();

          if (add) {
            r2.add(item);
          }
        }
      }
    }

    return res;
  }

  Future<void> generatePlayLists(
      int lessonType,
      Function(bool value,
              {String? label, String? msg, int? procent1, int? procent2})
          isLoading,
      bool isOnLine) async {
    var words = state.userLessonData!.getWords();
    var wordsForRepetition = state.userLessonData!.getWordsForRepetition();

    var repeatRepetitionsWordsInBaseMix = 2; // repeat repetition base words
    var repeatRepetitionsWordsAndPhrasesInAllMix = 2; // repeat repetition word
    var repeatRepetitionsWordPhrases = true;
    var wordPhasesInWord = 3; // phrases from word

    if (lessonType == LessonTypes.short.index) {
      repeatRepetitionsWordsInBaseMix =
          0; // repeat repetition word in base words mix
      repeatRepetitionsWordsAndPhrasesInAllMix =
          2; // repeat repetition word in all mix
      repeatRepetitionsWordPhrases = false; // no phrases from repetition words
      wordPhasesInWord = 0; // no phrases from fords
    }

    // remember lesson type depended settings in mixParam
    for (var mixParam in _articleByParamData!.mixParamsList) {
      mixParam.lessonType = lessonType;
      mixParam.repeatRepetitionsWordsInBaseMix =
          repeatRepetitionsWordsInBaseMix;
      mixParam.repeatRepetitionsWordsAndPhrasesInAllMix =
          repeatRepetitionsWordsAndPhrasesInAllMix;
      mixParam.repeatRepetitionsWordPhrases = repeatRepetitionsWordPhrases;
      mixParam.wordPhasesInWord = wordPhasesInWord;
    }

    // remove unnecessary phrases
    for (var word in words) {
      word.wordPhraseSelected =
          word.wordPhraseSelected!.take(wordPhasesInWord).toList();
    }

    // clear rep phrases if we will not use it
    for (var word in wordsForRepetition) {
      if (!repeatRepetitionsWordPhrases) {
        word.wordPhraseSelected = [];
      }
    }

    try {
      state.userLessonData!.playLists = await PlayListUtils.makePlayLists(
          words, _articleByParamData!, wordsForRepetition,
          (int val1, int val2) {
        isLoading(true, procent1: val1, procent2: val2);
      }, isOnLine);
    } catch (e) {
      rethrow;
    }

    var seconds = 0;
    for (var item in state.userLessonData!.playLists!) {
      seconds += item.totalSeconds;
    }

    state.userLessonData!.userLesson!.totalSeconds = seconds;
  }

  Future<void> tryToRestoreFromStore() async {
    debugPrint('PLP: tryToRestoreFromStore');

    if (!authState.isAuth) {
      debugPrint('PLP: NOT restore: no token or user');
      return;
    }

    final prefs = await SharedPreferences.getInstance();
    // if (!prefs.containsKey(_keyPref)) {
    //   if (preparationStep < 0) {
    //     preparationStep = PreparationStep.showUserData.index;
    //     debugPrint('PLP: NOTIFY4');
    //     notifyListeners();
    //   }
    //   debugPrint('PLP: NOT restore: no key');
    //   return;
    // }

    final val = prefs.getString(_keyPref) ?? '';
    if (val.isEmpty || val == "null") {
      // if (preparationStep < 0) {
      //   preparationStep = PreparationStep.showUserData.index;
      //   debugPrint('PLP: NOTIFY5');
      //   notifyListeners();
      // }
      debugPrint('PLP: NOT restore: $val');
      return;
    }

    final jsonValue = json.decode(val);
    _state =
        _state.copyWith(userLessonData: UserLessonData.fromJson(jsonValue));

    debugPrint('PLP: restore with SUCCESS');

    debugPrint('********* +++++ PlayListProvider notifyListeners4');
    notifyListeners();
    return;
  }

  Future<void> save({bool notify = true}) async {
    final data = json.encode(state.userLessonData);
    final prefs = await SharedPreferences.getInstance();
    prefs.setString(_keyPref, data);

    // save to local and outside db
    if (state.userLessonData?.userLesson != null) {
      // userLessonData!.userLesson!.jsonData = data;
      DatabaseService.userLessonUpdate(databaseState!.database,
          state.userLessonData!.userLesson!, databaseState!.userLessons);
    }

    debugPrint('PLP: SAVE PREFS with SUCCESS');
    if (notify) {
      debugPrint('PLP: NOTIFY7');
      debugPrint('********* +++++ PlayListProvider notifyListeners5');
      notifyListeners();
      // if (!_isDisposed) {
      //   notifyListeners();
      // }
    }
  }

  // @override
  // void dispose() {
  //   _player.dispose();
  //   _isDisposed = true;
  //   super.dispose();
  // }
}

enum PreparationStep { showUserData, showLessonsList, configureLesson }
