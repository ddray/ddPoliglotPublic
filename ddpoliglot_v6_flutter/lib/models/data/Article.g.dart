// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Article.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Article _$ArticleFromJson(Map<String, dynamic> json) => Article(
      json['articleID'] as int,
      json['userID'] as String?,
      json['name'] as String?,
      json['language'] as String?,
      json['languageTranslation'] as String?,
      json['textHashCode'] as String?,
      json['textSpeechFileName'] as String?,
      json['videoFileName'] as String?,
      (json['articlePhrases'] as List<dynamic>)
          .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
          .toList(),
      (json['articleActors'] as List<dynamic>)
          .map((e) => ArticleActor.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$ArticleToJson(Article instance) => <String, dynamic>{
      'articleID': instance.articleID,
      'userID': instance.userID,
      'name': instance.name,
      'language': instance.language,
      'languageTranslation': instance.languageTranslation,
      'textHashCode': instance.textHashCode,
      'textSpeechFileName': instance.textSpeechFileName,
      'videoFileName': instance.videoFileName,
      'articlePhrases': instance.articlePhrases,
      'articleActors': instance.articleActors,
    };
