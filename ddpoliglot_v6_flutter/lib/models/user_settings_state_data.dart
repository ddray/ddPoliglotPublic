import 'package:ddpoliglot_v6_flutter/models/data/user_language_level.dart';
import './data/Language.dart';
import 'package:json_annotation/json_annotation.dart';

part 'user_settings_state_data.g.dart';

@JsonSerializable()
class UserSettingsState {
  final Language? nativeLanguage;
  final Language? learnLanguage;
  final UserLanguageLevel? userLanguageLevel;
  final bool confSetWordGrade;
  final bool confClearWordGrade;
  final int wordsInLesson;
  final int lessonType;
  UserSettingsState(
      {this.nativeLanguage,
      this.learnLanguage,
      this.userLanguageLevel,
      this.confClearWordGrade = true,
      this.confSetWordGrade = true,
      this.wordsInLesson = 5,
      this.lessonType = 0});

  factory UserSettingsState.initial() {
    return UserSettingsState(
        nativeLanguage: Language(2, "ru", "ru-RU", "Russian", "ru"),
        learnLanguage: Language(1, "en", "en-US", "English(US, GB)", "en"),
        userLanguageLevel: null,
        confClearWordGrade: true,
        confSetWordGrade: true,
        wordsInLesson: 5,
        lessonType: 0);
  }

  bool get isEmpty {
    if (nativeLanguage == null ||
        learnLanguage == null ||
        userLanguageLevel == null) {
      return true;
    }

    return false;
  }

  factory UserSettingsState.fromJson(Map<String, dynamic> data) =>
      _$UserSettingsStateFromJson(data);

  Map<String, dynamic> toJson() => _$UserSettingsStateToJson(this);

  UserSettingsState copyWith({
    Language? nativeLanguage,
    Language? learnLanguage,
    UserLanguageLevel? userLanguageLevel,
    bool? confClearWordGrade,
    bool? confSetWordGrade,
    int? wordsInLesson,
    int? lessonType,
  }) =>
      UserSettingsState(
        nativeLanguage: nativeLanguage ?? this.nativeLanguage,
        learnLanguage: learnLanguage ?? this.learnLanguage,
        userLanguageLevel: userLanguageLevel ?? this.userLanguageLevel,
        confClearWordGrade: confClearWordGrade ?? this.confClearWordGrade,
        confSetWordGrade: confSetWordGrade ?? this.confSetWordGrade,
        wordsInLesson: wordsInLesson ?? this.wordsInLesson,
        lessonType: lessonType ?? this.lessonType,
      );
}
