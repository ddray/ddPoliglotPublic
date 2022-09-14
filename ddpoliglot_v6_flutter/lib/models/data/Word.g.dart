// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Word.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Word _$WordFromJson(Map<String, dynamic> json) => Word(
      json['wordID'] as int,
      json['languageID'] as int,
      json['language'] == null
          ? null
          : Language.fromJson(json['language'] as Map<String, dynamic>),
      json['text'] as String,
      json['rate'] as int,
      json['pronunciation'] as String?,
      json['hashCode'] as int,
      json['textSpeechFileName'] as String?,
      json['speachDuration'] as int,
      json['hashCodeSpeed1'] as int,
      json['textSpeechFileNameSpeed1'] as String?,
      json['speachDurationSpeed1'] as int,
      json['hashCodeSpeed2'] as int,
      json['textSpeechFileNameSpeed2'] as String?,
      json['speachDurationSpeed2'] as int,
      (json['wordPhraseWords'] as List<dynamic>?)
          ?.map((e) => WordPhraseWord.fromJson(e as Map<String, dynamic>))
          .toList(),
      json['wordTranslation'] == null
          ? null
          : WordTranslation.fromJson(
              json['wordTranslation'] as Map<String, dynamic>),
      json['wordUser'] == null
          ? null
          : WordUser.fromJson(json['wordUser'] as Map<String, dynamic>),
      (json['wordPhraseSelected'] as List<dynamic>?)
          ?.map((e) => WordPhrase.fromJson(e as Map<String, dynamic>))
          .toList(),
      json['oxfordLevel'] as int,
    );

Map<String, dynamic> _$WordToJson(Word instance) => <String, dynamic>{
      'wordID': instance.wordID,
      'languageID': instance.languageID,
      'language': instance.language,
      'text': instance.text,
      'rate': instance.rate,
      'pronunciation': instance.pronunciation,
      'hashCode': instance.hashCode,
      'textSpeechFileName': instance.textSpeechFileName,
      'speachDuration': instance.speachDuration,
      'hashCodeSpeed1': instance.hashCodeSpeed1,
      'textSpeechFileNameSpeed1': instance.textSpeechFileNameSpeed1,
      'speachDurationSpeed1': instance.speachDurationSpeed1,
      'hashCodeSpeed2': instance.hashCodeSpeed2,
      'textSpeechFileNameSpeed2': instance.textSpeechFileNameSpeed2,
      'speachDurationSpeed2': instance.speachDurationSpeed2,
      'wordPhraseWords': instance.wordPhraseWords,
      'wordTranslation': instance.wordTranslation,
      'wordUser': instance.wordUser,
      'wordPhraseSelected': instance.wordPhraseSelected,
      'oxfordLevel': instance.oxfordLevel,
    };
