// ignore_for_file: file_names, hash_and_equals, annotate_overrides
import 'package:json_annotation/json_annotation.dart';

part 'SpeechFile.g.dart';

@JsonSerializable()
class SpeechFile {
  int speechFileID;
  int hashCode;
  String speechFileName;
  int duration;
  int version;

  SpeechFile(
    this.speechFileID,
    this.hashCode,
    this.speechFileName,
    this.duration,
    this.version,
  );

  factory SpeechFile.fromJson(Map<String, dynamic> data) =>
      _$SpeechFileFromJson(data);

  Map<String, dynamic> toJson() => _$SpeechFileToJson(this);
}
