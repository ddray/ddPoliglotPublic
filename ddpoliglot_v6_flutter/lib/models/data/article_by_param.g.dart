// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'article_by_param.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ArticleByParam _$ArticleByParamFromJson(Map<String, dynamic> json) =>
    ArticleByParam(
      json['articleByParamID'] as int,
      json['userID'] as String?,
      json['nativeLanguageID'] as int,
      json['learnLanguageID'] as int,
      json['type'] as int,
      json['name'] as String,
      json['dataJson'] as String,
      json['isTemplate'] as bool,
      json['isShared'] as bool,
    );

Map<String, dynamic> _$ArticleByParamToJson(ArticleByParam instance) =>
    <String, dynamic>{
      'articleByParamID': instance.articleByParamID,
      'userID': instance.userID,
      'nativeLanguageID': instance.nativeLanguageID,
      'learnLanguageID': instance.learnLanguageID,
      'type': instance.type,
      'name': instance.name,
      'dataJson': instance.dataJson,
      'isTemplate': instance.isTemplate,
      'isShared': instance.isShared,
    };
