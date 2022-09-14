// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ArticleActor.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ArticleActor _$ArticleActorFromJson(Map<String, dynamic> json) => ArticleActor(
      json['articleActorID'] as int,
      json['keyString'] as String?,
      json['name'] as String?,
      json['role'] as int,
      json['defaultInRole'] as bool,
      json['language'] as String?,
      json['voiceName'] as String?,
      (json['voiceSpeed'] as num).toDouble(),
      (json['voicePitch'] as num).toDouble(),
      json['articleID'] as int,
      json['article'] == null
          ? null
          : Article.fromJson(json['article'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$ArticleActorToJson(ArticleActor instance) =>
    <String, dynamic>{
      'articleActorID': instance.articleActorID,
      'keyString': instance.keyString,
      'name': instance.name,
      'role': instance.role,
      'defaultInRole': instance.defaultInRole,
      'language': instance.language,
      'voiceName': instance.voiceName,
      'voiceSpeed': instance.voiceSpeed,
      'voicePitch': instance.voicePitch,
      'articleID': instance.articleID,
      'article': instance.article,
    };
