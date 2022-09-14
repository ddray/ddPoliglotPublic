// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'Language.g.dart';

@JsonSerializable()
class Language {
  int languageID;
  String name;
  String code;
  String codeFull;
  String shortName;

  Language(
    this.languageID,
    this.name,
    this.code,
    this.codeFull,
    this.shortName,
  );

  factory Language.fromJson(Map<String, dynamic> data) =>
      _$LanguageFromJson(data);

  Map<String, dynamic> toJson() => _$LanguageToJson(this);
}
