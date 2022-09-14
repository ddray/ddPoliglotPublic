// ignore_for_file: file_names
import 'dart:convert';

import 'package:ddpoliglot_v6_flutter/models/data/user_lesson_word.dart';
import 'package:json_annotation/json_annotation.dart';

part 'user_lesson.g.dart';

@JsonSerializable()
class UserLesson {
  int userLessonID;
  int languageID;
  String userID;
  int num;
  DateTime created;
  DateTime updated;
  DateTime finished;
  int totalSeconds;
  int learnSeconds;
  int lessonType;
  String? jsonData;

  List<UserLessonWord> userLessonWords;

  UserLesson(
      this.userLessonID,
      this.languageID,
      this.userID,
      this.num,
      this.created,
      this.userLessonWords,
      this.updated,
      this.finished,
      this.learnSeconds,
      this.totalSeconds,
      this.lessonType,
      this.jsonData);

  factory UserLesson.fromJson(Map<String, dynamic> data) =>
      _$UserLessonFromJson(data);

  Map<String, dynamic> toJson() => _$UserLessonToJson(this);
  Map<String, Object?> toDbJson() {
    return <String, dynamic>{
      'userLessonID': userLessonID,
      'languageID': languageID,
      'userID': userID,
      'num': num,
      'created': created.toIso8601String(),
      'updated': updated.toIso8601String(),
      'finished': finished.toIso8601String(),
      'totalSeconds': totalSeconds,
      'learnSeconds': learnSeconds,
      'lessonType': lessonType,
      'userLessonWordsJson': jsonEncode(userLessonWords),
      'jsonData': jsonData,
    };
  }

  factory UserLesson.fromDbJson(Map<String, Object?> json) => UserLesson(
        json['userLessonID'] as int,
        json['languageID'] as int,
        json['userID'] as String,
        json['num'] as int,
        DateTime.parse(json['created'] as String),
        (json['userLessonWordsJson'] as String).isNotEmpty
            ? (jsonDecode((json['userLessonWordsJson'] as String))
                    as List<dynamic>)
                .map((e) => UserLessonWord.fromJson(e as Map<String, dynamic>))
                .toList()
            : [],
        DateTime.parse(json['updated'] as String),
        DateTime.parse(json['finished'] as String),
        json['learnSeconds'] as int,
        json['totalSeconds'] as int,
        json['lessonType'] as int,
        json['jsonData'] as String?,
      );

  void copyFrom(UserLesson userLesson) {
    userLessonID = userLesson.userLessonID;
    languageID = userLesson.languageID;
    userID = userLesson.userID;
    num = userLesson.num;
    created = userLesson.created;
    userLessonWords = userLesson.userLessonWords;
    updated = userLesson.updated;
    finished = userLesson.finished;
    learnSeconds = userLesson.learnSeconds;
    totalSeconds = userLesson.totalSeconds;
    lessonType = userLesson.lessonType;
    jsonData = userLesson.jsonData;
  }
}
