// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'MixParamTextTemp.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MixParamTextTemp _$MixParamTextTempFromJson(Map<String, dynamic> json) =>
    MixParamTextTemp(
      json['mixParamTextTempID'] as int,
      json['text'] as String,
      json['keyTemp'] as String,
      json['learnLanguageID'] as int,
      json['nativeLanguageID'] as int,
    );

Map<String, dynamic> _$MixParamTextTempToJson(MixParamTextTemp instance) =>
    <String, dynamic>{
      'mixParamTextTempID': instance.mixParamTextTempID,
      'text': instance.text,
      'keyTemp': instance.keyTemp,
      'learnLanguageID': instance.learnLanguageID,
      'nativeLanguageID': instance.nativeLanguageID,
    };
