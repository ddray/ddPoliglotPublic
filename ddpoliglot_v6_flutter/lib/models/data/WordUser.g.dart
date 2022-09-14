// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'WordUser.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WordUser _$WordUserFromJson(Map<String, dynamic> json) => WordUser(
      json['wordUserID'] as int,
      json['wordID'] as int,
      json['word'] == null
          ? null
          : Word.fromJson(json['word'] as Map<String, dynamic>),
      json['userID'] as String,
      json['languageID'] as int,
      json['language'] == null
          ? null
          : Language.fromJson(json['language'] as Map<String, dynamic>),
      json['grade'] as int,
      json['lastRepeatInArticleByParamID'] as int,
      json['lastRepeatInLessonNum'] as int,
      json['lastRepeatWordPhraseId'] as int,
      json['sourceType'] as int,
    );

Map<String, dynamic> _$WordUserToJson(WordUser instance) => <String, dynamic>{
      'wordUserID': instance.wordUserID,
      'wordID': instance.wordID,
      'word': instance.word,
      'userID': instance.userID,
      'languageID': instance.languageID,
      'language': instance.language,
      'grade': instance.grade,
      'lastRepeatInArticleByParamID': instance.lastRepeatInArticleByParamID,
      'lastRepeatInLessonNum': instance.lastRepeatInLessonNum,
      'lastRepeatWordPhraseId': instance.lastRepeatWordPhraseId,
      'sourceType': instance.sourceType,
    };
