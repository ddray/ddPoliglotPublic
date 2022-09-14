// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'Log.g.dart';

@JsonSerializable()
class Log {
  int logID;
  String userID;
  String name;
  String message;
  String description;
  int type;
  DateTime created;

  Log(
    this.logID,
    this.userID,
    this.name,
    this.message,
    this.description,
    this.type,
    this.created,
  );

  factory Log.fromJson(Map<String, dynamic> data) => _$LogFromJson(data);

  Map<String, dynamic> toJson() => _$LogToJson(this);
}
