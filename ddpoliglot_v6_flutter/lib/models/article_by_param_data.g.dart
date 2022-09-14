// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'article_by_param_data.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ArticleByParamData _$ArticleByParamDataFromJson(Map<String, dynamic> json) =>
    ArticleByParamData(
      json['dialogText'] as String?,
      json['phrasesText'] as String?,
      json['phrasesTranslationText'] as String?,
      (json['mixParamsList'] as List<dynamic>)
          .map((e) => MixParam.fromJson(e as Map<String, dynamic>))
          .toList(),
      json['baseName'] as String?,
      json['firstDictorPhrases'] as String?,
      json['beforeFinishDictorPhrases'] as String?,
      json['finishDictorPhrases'] as String?,
      json['maxWordsToRepeatForGeneration'] as int?,
    );

Map<String, dynamic> _$ArticleByParamDataToJson(ArticleByParamData instance) =>
    <String, dynamic>{
      'dialogText': instance.dialogText,
      'phrasesText': instance.phrasesText,
      'phrasesTranslationText': instance.phrasesTranslationText,
      'mixParamsList': instance.mixParamsList,
      'baseName': instance.baseName,
      'firstDictorPhrases': instance.firstDictorPhrases,
      'beforeFinishDictorPhrases': instance.beforeFinishDictorPhrases,
      'finishDictorPhrases': instance.finishDictorPhrases,
      'maxWordsToRepeatForGeneration': instance.maxWordsToRepeatForGeneration,
    };
