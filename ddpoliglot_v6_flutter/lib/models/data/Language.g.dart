// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Language.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Language _$LanguageFromJson(Map<String, dynamic> json) => Language(
      json['languageID'] as int,
      json['name'] as String,
      json['code'] as String,
      json['codeFull'] as String,
      json['shortName'] as String,
    );

Map<String, dynamic> _$LanguageToJson(Language instance) => <String, dynamic>{
      'languageID': instance.languageID,
      'name': instance.name,
      'code': instance.code,
      'codeFull': instance.codeFull,
      'shortName': instance.shortName,
    };
