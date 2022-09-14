// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'SpeechFile.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SpeechFile _$SpeechFileFromJson(Map<String, dynamic> json) => SpeechFile(
      json['speechFileID'] as int,
      json['hashCode'] as int,
      json['speechFileName'] as String,
      json['duration'] as int,
      json['version'] as int,
    );

Map<String, dynamic> _$SpeechFileToJson(SpeechFile instance) =>
    <String, dynamic>{
      'speechFileID': instance.speechFileID,
      'hashCode': instance.hashCode,
      'speechFileName': instance.speechFileName,
      'duration': instance.duration,
      'version': instance.version,
    };
