// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'play_list.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlayList _$PlayListFromJson(Map<String, dynamic> json) => PlayList(
      json['guid'] as String,
      json['name'] as String,
      json['active'] as bool,
      json['isCurrent'] as bool?,
      (json['items'] as List<dynamic>)
          .map((e) => PlayListItem.fromJson(e as Map<String, dynamic>))
          .toList(),
      json['totalSeconds'] as int,
      json['learnSeconds'] as int,
    );

Map<String, dynamic> _$PlayListToJson(PlayList instance) => <String, dynamic>{
      'guid': instance.guid,
      'name': instance.name,
      'active': instance.active,
      'isCurrent': instance.isCurrent,
      'items': instance.items,
      'totalSeconds': instance.totalSeconds,
      'learnSeconds': instance.learnSeconds,
    };
