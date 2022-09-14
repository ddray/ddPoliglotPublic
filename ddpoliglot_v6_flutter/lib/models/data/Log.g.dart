// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Log.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Log _$LogFromJson(Map<String, dynamic> json) => Log(
      json['logID'] as int,
      json['userID'] as String,
      json['name'] as String,
      json['message'] as String,
      json['description'] as String,
      json['type'] as int,
      DateTime.parse(json['created'] as String),
    );

Map<String, dynamic> _$LogToJson(Log instance) => <String, dynamic>{
      'logID': instance.logID,
      'userID': instance.userID,
      'name': instance.name,
      'message': instance.message,
      'description': instance.description,
      'type': instance.type,
      'created': instance.created.toIso8601String(),
    };
