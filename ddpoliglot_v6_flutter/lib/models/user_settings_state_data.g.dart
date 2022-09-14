// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_settings_state_data.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserSettingsState _$UserSettingsStateFromJson(Map<String, dynamic> json) =>
    UserSettingsState(
      nativeLanguage: json['nativeLanguage'] == null
          ? null
          : Language.fromJson(json['nativeLanguage'] as Map<String, dynamic>),
      learnLanguage: json['learnLanguage'] == null
          ? null
          : Language.fromJson(json['learnLanguage'] as Map<String, dynamic>),
      userLanguageLevel: json['userLanguageLevel'] == null
          ? null
          : UserLanguageLevel.fromJson(
              json['userLanguageLevel'] as Map<String, dynamic>),
      confClearWordGrade: json['confClearWordGrade'] as bool? ?? true,
      confSetWordGrade: json['confSetWordGrade'] as bool? ?? true,
      wordsInLesson: json['wordsInLesson'] as int? ?? 5,
      lessonType: json['lessonType'] as int? ?? 0,
    );

Map<String, dynamic> _$UserSettingsStateToJson(UserSettingsState instance) =>
    <String, dynamic>{
      'nativeLanguage': instance.nativeLanguage,
      'learnLanguage': instance.learnLanguage,
      'userLanguageLevel': instance.userLanguageLevel,
      'confSetWordGrade': instance.confSetWordGrade,
      'confClearWordGrade': instance.confClearWordGrade,
      'wordsInLesson': instance.wordsInLesson,
      'lessonType': instance.lessonType,
    };
