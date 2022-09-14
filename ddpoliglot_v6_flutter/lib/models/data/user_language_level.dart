// ignore_for_file: file_names, hash_and_equals, annotate_overrides
import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'user_language_level.g.dart';

@JsonSerializable()
class UserLanguageLevel extends Equatable {
  final int userLanguageLevelID;
  final int languageID;
  final String userID;
  final int level;

  const UserLanguageLevel(
    this.userLanguageLevelID,
    this.languageID,
    this.userID,
    this.level,
  );

  factory UserLanguageLevel.fromJson(Map<String, dynamic> data) =>
      _$UserLanguageLevelFromJson(data);

  Map<String, dynamic> toJson() => _$UserLanguageLevelToJson(this);

  UserLanguageLevel copyWith({
    int? userLanguageLevelID,
    int? languageID,
    String? userID,
    int? level,
  }) {
    return UserLanguageLevel(
      userLanguageLevelID ?? this.userLanguageLevelID,
      languageID ?? this.languageID,
      userID ?? this.userID,
      level ?? this.level,
    );
  }

  Map<String, dynamic> toMap() {
    final result = <String, dynamic>{};

    result.addAll({'userLanguageLevelID': userLanguageLevelID});
    result.addAll({'languageID': languageID});
    result.addAll({'userID': userID});
    result.addAll({'level': level});

    return result;
  }

  factory UserLanguageLevel.fromMap(Map<String, dynamic> map) {
    return UserLanguageLevel(
      map['userLanguageLevelID']?.toInt() ?? 0,
      map['languageID']?.toInt() ?? 0,
      map['userID'] ?? '',
      map['level']?.toInt() ?? 0,
    );
  }

  // String toJson() => json.encode(toMap());

  // factory UserLanguageLevel.fromJson(String source) =>
  //     UserLanguageLevel.fromMap(json.decode(source));

  @override
  String toString() {
    return 'UserLanguageLevel(userLanguageLevelID: $userLanguageLevelID, languageID: $languageID, userID: $userID, level: $level)';
  }

  @override
  List<Object> get props => [userLanguageLevelID, languageID, userID, level];
}
