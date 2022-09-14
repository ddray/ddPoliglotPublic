// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'WordPhrase.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WordPhrase _$WordPhraseFromJson(Map<String, dynamic> json) => WordPhrase(
      json['wordPhraseID'] as int,
      json['languageID'] as int,
      json['language'] == null
          ? null
          : Language.fromJson(json['language'] as Map<String, dynamic>),
      json['text'] as String,
      json['hashCode'] as int,
      json['textSpeechFileName'] as String?,
      json['speachDuration'] as int,
      json['hashCodeSpeed1'] as int,
      json['textSpeechFileNameSpeed1'] as String?,
      json['speachDurationSpeed1'] as int,
      json['hashCodeSpeed2'] as int,
      json['textSpeechFileNameSpeed2'] as String?,
      json['speachDurationSpeed2'] as int,
      json['sourceType'] as int,
      json['wordsUsed'] as String?,
      (json['wordPhraseWords'] as List<dynamic>)
          .map((e) => WordPhraseWord.fromJson(e as Map<String, dynamic>))
          .toList(),
      json['wordPhraseTranslation'] == null
          ? null
          : WordPhraseTranslation.fromJson(
              json['wordPhraseTranslation'] as Map<String, dynamic>),
      json['phraseOrderInCurrentWord'] as int,
      json['currentWordID'] as int,
    );

Map<String, dynamic> _$WordPhraseToJson(WordPhrase instance) =>
    <String, dynamic>{
      'wordPhraseID': instance.wordPhraseID,
      'languageID': instance.languageID,
      'language': instance.language,
      'text': instance.text,
      'hashCode': instance.hashCode,
      'textSpeechFileName': instance.textSpeechFileName,
      'speachDuration': instance.speachDuration,
      'hashCodeSpeed1': instance.hashCodeSpeed1,
      'textSpeechFileNameSpeed1': instance.textSpeechFileNameSpeed1,
      'speachDurationSpeed1': instance.speachDurationSpeed1,
      'hashCodeSpeed2': instance.hashCodeSpeed2,
      'textSpeechFileNameSpeed2': instance.textSpeechFileNameSpeed2,
      'speachDurationSpeed2': instance.speachDurationSpeed2,
      'sourceType': instance.sourceType,
      'wordsUsed': instance.wordsUsed,
      'wordPhraseWords': instance.wordPhraseWords,
      'wordPhraseTranslation': instance.wordPhraseTranslation,
      'phraseOrderInCurrentWord': instance.phraseOrderInCurrentWord,
      'currentWordID': instance.currentWordID,
    };
