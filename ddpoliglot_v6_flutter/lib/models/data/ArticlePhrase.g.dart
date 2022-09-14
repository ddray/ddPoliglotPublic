// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'ArticlePhrase.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ArticlePhrase _$ArticlePhraseFromJson(Map<String, dynamic> json) =>
    ArticlePhrase(
      json['articlePhraseID'] as int,
      json['keyString'] as String?,
      json['articleID'] as int,
      json['article'] == null
          ? null
          : Article.fromJson(json['article'] as Map<String, dynamic>),
      json['text'] as String,
      json['hashCode'] as int,
      json['textSpeechFileName'] as String?,
      json['speachDuration'] as int,
      json['pause'] as int,
      json['articleActorID'] as int,
      json['articleActor'] == null
          ? null
          : ArticleActor.fromJson(json['articleActor'] as Map<String, dynamic>),
      json['trText'] as String?,
      json['trHashCode'] as int,
      json['trTextSpeechFileName'] as String?,
      json['trSpeachDuration'] as int,
      json['trPause'] as int,
      json['trArticleActorID'] as int,
      json['trArticleActor'] == null
          ? null
          : ArticleActor.fromJson(
              json['trArticleActor'] as Map<String, dynamic>),
      json['orderNum'] as int,
      json['activityType'] as int,
      json['type'] as int,
      json['parentKeyString'] as String?,
      json['childrenType'] as String?,
      json['hasChildren'] as bool,
      json['childrenClosed'] as bool,
      json['selected'] as bool,
      json['silent'] as bool,
    );

Map<String, dynamic> _$ArticlePhraseToJson(ArticlePhrase instance) =>
    <String, dynamic>{
      'articlePhraseID': instance.articlePhraseID,
      'keyString': instance.keyString,
      'articleID': instance.articleID,
      'article': instance.article,
      'text': instance.text,
      'hashCode': instance.hashCode,
      'textSpeechFileName': instance.textSpeechFileName,
      'speachDuration': instance.speachDuration,
      'pause': instance.pause,
      'articleActorID': instance.articleActorID,
      'articleActor': instance.articleActor,
      'trText': instance.trText,
      'trHashCode': instance.trHashCode,
      'trTextSpeechFileName': instance.trTextSpeechFileName,
      'trSpeachDuration': instance.trSpeachDuration,
      'trPause': instance.trPause,
      'trArticleActorID': instance.trArticleActorID,
      'trArticleActor': instance.trArticleActor,
      'orderNum': instance.orderNum,
      'activityType': instance.activityType,
      'type': instance.type,
      'parentKeyString': instance.parentKeyString,
      'childrenType': instance.childrenType,
      'hasChildren': instance.hasChildren,
      'childrenClosed': instance.childrenClosed,
      'selected': instance.selected,
      'silent': instance.silent,
    };
