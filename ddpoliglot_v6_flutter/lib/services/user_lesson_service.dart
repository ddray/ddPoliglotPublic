import 'dart:convert';

import 'package:ddpoliglot_v6_flutter/models/data/Word.dart';
import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:flutter/material.dart';

import '../utils/http_utils.dart';

class UserLessonService {
  static Future<Map<String, dynamic>> createNext(List<DictWord> selectedWords,
      List<DictWord> wordsForRepetition, int lessonType, int lessonNum) async {
    final wordIDs = selectedWords.map((e) => e.wordID).toList();
    final repeatWordIds = wordsForRepetition.map((e) => e.wordID).toList();

    final body = json.encode({
      'wordIDs': wordIDs,
      'repeatWordIds': repeatWordIds,
      'lessonType': lessonType,
      'lessonNum': lessonNum,
    });

    final extractedData = await HttpUtils.post('UserLesson/CreateNext', body);

    var words = (extractedData["words"] as List<dynamic>).map((e) {
      var w = Word.fromJson(e as Map<String, dynamic>);
      return w;
    }).toList();

    var repetition =
        (extractedData["wordsForRepetition"] as List<dynamic>).map((e) {
      var w = Word.fromJson(e as Map<String, dynamic>);
      return w;
    }).toList();

    var userLesson = UserLesson.fromJson(
        extractedData["userLesson"] as Map<String, dynamic>);

    return {'userLesson': userLesson, 'words': words, 'repetition': repetition};
  }

  static Future<List<UserLesson>> getAll() async {
    final extractedData = await HttpUtils.get('UserLesson/GetAll');
    var userLessons = (extractedData["userLessons"] as List<dynamic>).map((e) {
      var w = UserLesson.fromJson(e as Map<String, dynamic>);
      return w;
    }).toList();

    debugPrint('get: $userLessons');
    return userLessons;
  }

  static Future<UserLesson?> update(UserLesson userLesson) async {
    if (!(await HttpUtils.isOnLine())) {
      return null;
    }

    final body = json.encode(userLesson);
    final extractedData = await HttpUtils.post('UserLesson/Update', body);
    var result = extractedData == null
        ? null
        : UserLesson.fromJson(extractedData as Map<String, dynamic>);
    debugPrint('update: $result');
    return result;
  }
}
