// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'WordTranslation.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WordTranslation _$WordTranslationFromJson(Map<String, dynamic> json) =>
    WordTranslation(
      json['wordTranslationID'] as int,
      json['wordID'] as int,
      json['word'] == null
          ? null
          : Word.fromJson(json['word'] as Map<String, dynamic>),
      json['languageID'] as int,
      json['language'] == null
          ? null
          : Language.fromJson(json['language'] as Map<String, dynamic>),
      json['text'] as String,
      json['hashCode'] as int,
      json['textSpeechFileName'] as String?,
      json['speachDuration'] as int,
      json['readyForLessonPhrasiesCnt'] as int,
    );

Map<String, dynamic> _$WordTranslationToJson(WordTranslation instance) =>
    <String, dynamic>{
      'wordTranslationID': instance.wordTranslationID,
      'wordID': instance.wordID,
      'word': instance.word,
      'languageID': instance.languageID,
      'language': instance.language,
      'text': instance.text,
      'hashCode': instance.hashCode,
      'textSpeechFileName': instance.textSpeechFileName,
      'speachDuration': instance.speachDuration,
      'readyForLessonPhrasiesCnt': instance.readyForLessonPhrasiesCnt,
    };
