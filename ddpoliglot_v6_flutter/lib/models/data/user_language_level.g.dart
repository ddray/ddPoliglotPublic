// // GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_language_level.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserLanguageLevel _$UserLanguageLevelFromJson(Map<String, dynamic> json) =>
    UserLanguageLevel(
      json['userLanguageLevelID'] as int,
      json['languageID'] as int,
      json['userID'] as String,
      json['level'] as int,
    );

Map<String, dynamic> _$UserLanguageLevelToJson(UserLanguageLevel instance) =>
    <String, dynamic>{
      'userLanguageLevelID': instance.userLanguageLevelID,
      'languageID': instance.languageID,
      'userID': instance.userID,
      'level': instance.level,
    };
