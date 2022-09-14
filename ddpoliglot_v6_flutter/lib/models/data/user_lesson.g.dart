// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_lesson.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserLesson _$UserLessonFromJson(Map<String, dynamic> json) => UserLesson(
      json['userLessonID'] as int,
      json['languageID'] as int,
      json['userID'] as String,
      json['num'] as int,
      DateTime.parse(json['created'] as String),
      (json['userLessonWords'] as List<dynamic>)
          .map((e) => UserLessonWord.fromJson(e as Map<String, dynamic>))
          .toList(),
      DateTime.parse(json['updated'] as String),
      DateTime.parse(json['finished'] as String),
      json['learnSeconds'] as int,
      json['totalSeconds'] as int,
      json['lessonType'] as int,
      json['jsonData'] as String?,
    );

Map<String, dynamic> _$UserLessonToJson(UserLesson instance) =>
    <String, dynamic>{
      'userLessonID': instance.userLessonID,
      'languageID': instance.languageID,
      'userID': instance.userID,
      'num': instance.num,
      'created': instance.created.toIso8601String(),
      'updated': instance.updated.toIso8601String(),
      'finished': instance.finished.toIso8601String(),
      'totalSeconds': instance.totalSeconds,
      'learnSeconds': instance.learnSeconds,
      'lessonType': instance.lessonType,
      'jsonData': instance.jsonData,
      'userLessonWords': instance.userLessonWords,
    };
