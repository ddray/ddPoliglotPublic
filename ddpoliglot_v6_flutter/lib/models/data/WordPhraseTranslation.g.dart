// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'WordPhraseTranslation.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WordPhraseTranslation _$WordPhraseTranslationFromJson(
        Map<String, dynamic> json) =>
    WordPhraseTranslation(
      json['wordPhraseTranslationID'] as int,
      json['wordPhraseID'] as int,
      json['wordPhrase'] == null
          ? null
          : WordPhrase.fromJson(json['wordPhrase'] as Map<String, dynamic>),
      json['languageID'] as int,
      json['language'] == null
          ? null
          : Language.fromJson(json['language'] as Map<String, dynamic>),
      json['text'] as String,
      json['hashCode'] as int,
      json['textSpeechFileName'] as String?,
      json['speachDuration'] as int,
    );

Map<String, dynamic> _$WordPhraseTranslationToJson(
        WordPhraseTranslation instance) =>
    <String, dynamic>{
      'wordPhraseTranslationID': instance.wordPhraseTranslationID,
      'wordPhraseID': instance.wordPhraseID,
      'wordPhrase': instance.wordPhrase,
      'languageID': instance.languageID,
      'language': instance.language,
      'text': instance.text,
      'hashCode': instance.hashCode,
      'textSpeechFileName': instance.textSpeechFileName,
      'speachDuration': instance.speachDuration,
    };
