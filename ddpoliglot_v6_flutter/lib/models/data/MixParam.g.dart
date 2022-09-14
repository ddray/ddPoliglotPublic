// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'MixParam.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MixParam _$MixParamFromJson(Map<String, dynamic> json) => MixParam(
      (json['mixItems'] as List<dynamic>)
          .map((e) => MixItem.fromJson(e as Map<String, dynamic>))
          .toList(),
      json['trFirst'] as bool,
      json['active'] as bool,
      json['trActive'] as bool,
      json['phrasesMixType'] as int,
      json['repeat'] as int,
      json['trRepeat'] as int,
      json['repeatOrder'] as int,
      json['trRepeatOrder'] as int,
      json['addSlowInRepeatOrder'] as bool,
      json['addSlow2InRepeatOrder'] as bool,
      json['trAddSlowInRepeatOrder'] as bool,
      json['trAddSlow2InRepeatOrder'] as bool,
      json['repeatBaseWord'] as int,
      json['trRepeatBaseWord'] as int,
      (json['firstDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['firstBeforeDialogDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['beforeByOrderMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['insideByOrderMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['beforeBaseWordsDirMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['insideBaseWordsDirMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['beforeBaseWordsRevMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['insideBaseWordsRevMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['beforeAllDirMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['insideAllDirMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['beforeAllRevMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['insideAllRevMixDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['beforeFinishDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),
      (json['finishDictorPhrasesWithAudioW'] as List<dynamic>)
          .map((e) => (e as List<dynamic>)
              .map((e) => (e as List<dynamic>)
                  .map((e) => APhWr.fromJson(e as Map<String, dynamic>))
                  .toList())
              .toList())
          .toList(),

      // (json['firstDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['firstBeforeDialogDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['beforeByOrderMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['insideByOrderMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['beforeBaseWordsDirMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['insideBaseWordsDirMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['beforeBaseWordsRevMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['insideBaseWordsRevMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['beforeAllDirMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['insideAllDirMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['beforeAllRevMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['insideAllRevMixDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['beforeFinishDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
      // (json['finishDictorPhrasesWithAudio'] as List<dynamic>)
      //     .map((e) => (e as List<dynamic>)
      //         .map((e) => (e as List<dynamic>)
      //             .map((e) => ArticlePhrase.fromJson(e as Map<String, dynamic>))
      //             .toList())
      //         .toList())
      //     .toList(),
    );

Map<String, dynamic> _$MixParamToJson(MixParam instance) => <String, dynamic>{
      'mixItems': instance.mixItems,
      'trFirst': instance.trFirst,
      'active': instance.active,
      'trActive': instance.trActive,
      'phrasesMixType': instance.phrasesMixType,
      'repeat': instance.repeat,
      'trRepeat': instance.trRepeat,
      'repeatOrder': instance.repeatOrder,
      'trRepeatOrder': instance.trRepeatOrder,
      'addSlowInRepeatOrder': instance.addSlowInRepeatOrder,
      'addSlow2InRepeatOrder': instance.addSlow2InRepeatOrder,
      'trAddSlowInRepeatOrder': instance.trAddSlowInRepeatOrder,
      'trAddSlow2InRepeatOrder': instance.trAddSlow2InRepeatOrder,
      'repeatBaseWord': instance.repeatBaseWord,
      'trRepeatBaseWord': instance.trRepeatBaseWord,
      'firstDictorPhrasesWithAudioW': instance.firstDictorPhrasesWithAudioW,
      'firstBeforeDialogDictorPhrasesWithAudioW':
          instance.firstBeforeDialogDictorPhrasesWithAudioW,
      'beforeByOrderMixDictorPhrasesWithAudioW':
          instance.beforeByOrderMixDictorPhrasesWithAudioW,
      'insideByOrderMixDictorPhrasesWithAudioW':
          instance.insideByOrderMixDictorPhrasesWithAudioW,
      'beforeBaseWordsDirMixDictorPhrasesWithAudioW':
          instance.beforeBaseWordsDirMixDictorPhrasesWithAudioW,
      'insideBaseWordsDirMixDictorPhrasesWithAudioW':
          instance.insideBaseWordsDirMixDictorPhrasesWithAudioW,
      'beforeBaseWordsRevMixDictorPhrasesWithAudioW':
          instance.beforeBaseWordsRevMixDictorPhrasesWithAudioW,
      'insideBaseWordsRevMixDictorPhrasesWithAudioW':
          instance.insideBaseWordsRevMixDictorPhrasesWithAudioW,
      'beforeAllDirMixDictorPhrasesWithAudioW':
          instance.beforeAllDirMixDictorPhrasesWithAudioW,
      'insideAllDirMixDictorPhrasesWithAudioW':
          instance.insideAllDirMixDictorPhrasesWithAudioW,
      'beforeAllRevMixDictorPhrasesWithAudioW':
          instance.beforeAllRevMixDictorPhrasesWithAudioW,
      'insideAllRevMixDictorPhrasesWithAudioW':
          instance.insideAllRevMixDictorPhrasesWithAudioW,
      'beforeFinishDictorPhrasesWithAudioW':
          instance.beforeFinishDictorPhrasesWithAudioW,
      'finishDictorPhrasesWithAudioW': instance.finishDictorPhrasesWithAudioW,
    };
