// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_lesson_data.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserLessonData _$UserLessonDataFromJson(Map<String, dynamic> json) =>
    UserLessonData(
      userLesson: json['userLesson'] == null
          ? null
          : UserLesson.fromJson(json['userLesson'] as Map<String, dynamic>),
      playLists: (json['playLists'] as List<dynamic>?)
          ?.map((e) => PlayList.fromJson(e as Map<String, dynamic>))
          .toList(),
      startPlay: json['startPlay'] == null
          ? null
          : DateTime.parse(json['startPlay'] as String),
      finished: json['finished'] as bool? ?? false,
      currentPlayListIndex: json['currentPlayListIndex'] as int? ?? 0,
      currentItemNum: json['currentItemNum'] as int? ?? 0,
      runLessonNum: json['runLessonNum'] as int? ?? 0,
    );

Map<String, dynamic> _$UserLessonDataToJson(UserLessonData instance) =>
    <String, dynamic>{
      'finished': instance.finished,
      'playLists': instance.playLists,
      'userLesson': instance.userLesson,
      'currentPlayListIndex': instance.currentPlayListIndex,
      'currentItemNum': instance.currentItemNum,
      'startPlay': instance.startPlay?.toIso8601String(),
      'runLessonNum': instance.runLessonNum,
    };
