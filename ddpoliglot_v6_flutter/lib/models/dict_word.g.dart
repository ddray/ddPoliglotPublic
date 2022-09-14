// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'dict_word.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DictWord _$DictWordFromJson(Map<String, dynamic> json) => DictWord(
      json['wordID'] as int,
      json['text'] as String,
      json['rate'] as int,
      json['pronunciation'] as String?,
      json['translation'] as String?,
      json['grade'] as int,
      json['oxfordLevel'] as int,
      json['wordUserID'] as int,
      json['lastRepeatInArticleByParamID'] as int,
      json['lastRepeatInLessonNum'] as int,
      json['lastRepeatWordPhraseId'] as int,
      json['sourceType'] as int,
      json['textClear'] as String,
      json['readyForLesson'] as int,
    );

Map<String, dynamic> _$DictWordToJson(DictWord instance) => <String, dynamic>{
      'wordID': instance.wordID,
      'text': instance.text,
      'rate': instance.rate,
      'pronunciation': instance.pronunciation,
      'translation': instance.translation,
      'oxfordLevel': instance.oxfordLevel,
      'wordUserID': instance.wordUserID,
      'grade': instance.grade,
      'lastRepeatInArticleByParamID': instance.lastRepeatInArticleByParamID,
      'lastRepeatInLessonNum': instance.lastRepeatInLessonNum,
      'lastRepeatWordPhraseId': instance.lastRepeatWordPhraseId,
      'sourceType': instance.sourceType,
      'textClear': instance.textClear,
      'readyForLesson': instance.readyForLesson,
    };
