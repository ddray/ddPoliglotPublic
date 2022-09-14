// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'word_selected.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WordSelected _$WordSelectedFromJson(Map<String, dynamic> json) => WordSelected(
      Word.fromJson(json['word'] as Map<String, dynamic>),
      (json['phraseWords'] as List<dynamic>)
          .map((e) => WordPhrase.fromJson(e as Map<String, dynamic>))
          .toList(),
      (json['phraseWordsSelected'] as List<dynamic>)
          .map((e) => WordPhrase.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$WordSelectedToJson(WordSelected instance) =>
    <String, dynamic>{
      'word': instance.word,
      'phraseWords': instance.phraseWords,
      'phraseWordsSelected': instance.phraseWordsSelected,
    };
