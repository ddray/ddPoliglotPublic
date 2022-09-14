// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'WordPhraseWord.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WordPhraseWord _$WordPhraseWordFromJson(Map<String, dynamic> json) =>
    WordPhraseWord(
      json['wordPhraseWordID'] as int,
      json['wordID'] as int,
      json['word'] == null
          ? null
          : Word.fromJson(json['word'] as Map<String, dynamic>),
      json['wordPhraseID'] as int,
      json['wordPhrase'] == null
          ? null
          : WordPhrase.fromJson(json['wordPhrase'] as Map<String, dynamic>),
      json['phraseOrder'] as int,
    );

Map<String, dynamic> _$WordPhraseWordToJson(WordPhraseWord instance) =>
    <String, dynamic>{
      'wordPhraseWordID': instance.wordPhraseWordID,
      'wordID': instance.wordID,
      'word': instance.word,
      'wordPhraseID': instance.wordPhraseID,
      'wordPhrase': instance.wordPhrase,
      'phraseOrder': instance.phraseOrder,
    };
