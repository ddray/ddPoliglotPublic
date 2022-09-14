// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_lesson_word.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserLessonWord _$UserLessonWordFromJson(Map<String, dynamic> json) =>
    UserLessonWord(
      json['userLessonWordID'] as int,
      json['userLessonID'] as int,
      json['wordID'] as int,
      json['word'] == null
          ? null
          : Word.fromJson(json['word'] as Map<String, dynamic>),
      json['wordType'] as int,
    )..dictWord = json['dictWord'] == null
        ? null
        : DictWord.fromJson(json['dictWord'] as Map<String, dynamic>);

Map<String, dynamic> _$UserLessonWordToJson(UserLessonWord instance) =>
    <String, dynamic>{
      'userLessonWordID': instance.userLessonWordID,
      'userLessonID': instance.userLessonID,
      'wordID': instance.wordID,
      'word': instance.word,
      'wordType': instance.wordType,
      'dictWord': instance.dictWord,
    };
