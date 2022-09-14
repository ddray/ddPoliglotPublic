// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'MixItem.g.dart';

@JsonSerializable()
class MixItem {
  int mixItemID;
  int mixParamID;
  String? keyString;
  String? source;
  String? inDict;
  String? inContext;
  String? childrenType;
  bool endPhrase;
  bool pretext;
  int orderNum;
  bool baseWord;

  MixItem(
    this.mixItemID,
    this.mixParamID,
    this.keyString,
    this.source,
    this.inDict,
    this.inContext,
    this.childrenType,
    this.endPhrase,
    this.pretext,
    this.orderNum,
    this.baseWord,
  );

  factory MixItem.fromJson(Map<String, dynamic> data) =>
      _$MixItemFromJson(data);

  Map<String, dynamic> toJson() => _$MixItemToJson(this);
}
