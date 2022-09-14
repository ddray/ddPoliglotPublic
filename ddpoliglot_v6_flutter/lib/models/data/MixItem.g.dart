// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'MixItem.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MixItem _$MixItemFromJson(Map<String, dynamic> json) => MixItem(
      json['mixItemID'] as int,
      json['mixParamID'] as int,
      json['keyString'] as String?,
      json['source'] as String?,
      json['inDict'] as String?,
      json['inContext'] as String?,
      json['childrenType'] as String?,
      json['endPhrase'] as bool,
      json['pretext'] as bool,
      json['orderNum'] as int,
      json['baseWord'] as bool,
    );

Map<String, dynamic> _$MixItemToJson(MixItem instance) => <String, dynamic>{
      'mixItemID': instance.mixItemID,
      'mixParamID': instance.mixParamID,
      'keyString': instance.keyString,
      'source': instance.source,
      'inDict': instance.inDict,
      'inContext': instance.inContext,
      'childrenType': instance.childrenType,
      'endPhrase': instance.endPhrase,
      'pretext': instance.pretext,
      'orderNum': instance.orderNum,
      'baseWord': instance.baseWord,
    };
