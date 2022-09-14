import 'dart:convert';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:flutter/cupertino.dart';

import '../utils/http_utils.dart';
import '../models/data/WordUser.dart';

class WordUserService {
  static Future<WordUser> updateWU(WordUser wordUser) async {
    final extractedData =
        await HttpUtils.post('WordUser/Update1', jsonEncode(wordUser));
    var item = WordUser.fromJson(extractedData as Map<String, dynamic>);
    return item;
  }

  static Future<WordUser> setWordGrade(int grade, int wordID, int wordUserID,
      DatabaseProvider databaseProvider) async {
    // update data in local database
    final db = await databaseProvider.state.database;
    var dictWord = DictWord.fromJson(
        (await db!.query('dictWords', where: 'wordID = ?', whereArgs: [wordID]))
            .first);
    debugPrint('grade from db ${dictWord.grade}');
    dictWord.grade = grade;
    var dictWordJson = dictWord.toJson();
    debugPrint('grade update db $dictWordJson');
    debugPrint('grade update wordID $wordID');

    await db.update('dictWords', dictWordJson,
        where: 'wordID = ?', whereArgs: [wordID]);

    var wordUser = WordUser(
        wordUserID,
        wordID,
        null,
        databaseProvider.authState.userId,
        databaseProvider.userSettingsStateData!.learnLanguage!.languageID,
        null,
        grade,
        0,
        0,
        0,
        0);
    var item = await updateWU(wordUser);
    dictWord.wordUserID = item.wordUserID;
    dictWordJson = dictWord.toJson();
    // debugPrint('grade2 update db $dictWordJson');
    // debugPrint('grade2 update wordID $wordID');

    await db.update('dictWords', dictWord.toJson(),
        where: 'wordID = ?', whereArgs: [wordID]);

    // var dvJson =
    //     (await db.query('dictWords', where: 'wordID = ?', whereArgs: [wordID]))
    //         .first;
    // debugPrint('grade from db22 ${dvJson}');
    // var dictWordNew = DictWord.fromJson(dvJson);
    // debugPrint('grade from db2 ${dictWordNew.grade}');

    return item;
  }
}
