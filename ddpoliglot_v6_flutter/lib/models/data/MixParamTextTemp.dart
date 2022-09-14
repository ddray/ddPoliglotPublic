// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'MixParamTextTemp.g.dart';

@JsonSerializable()
class MixParamTextTemp {
  int mixParamTextTempID;
  String text;
  String keyTemp;
  int learnLanguageID;
  int nativeLanguageID;

  MixParamTextTemp(
    this.mixParamTextTempID,
    this.text,
    this.keyTemp,
    this.learnLanguageID,
    this.nativeLanguageID,
  );

  factory MixParamTextTemp.fromJson(Map<String, dynamic> data) =>
      _$MixParamTextTempFromJson(data);

  Map<String, dynamic> toJson() => _$MixParamTextTempToJson(this);
}
