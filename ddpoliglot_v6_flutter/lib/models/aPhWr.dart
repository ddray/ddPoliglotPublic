// ignore_for_file: file_names
import '../models/play_list_item.dart';
import 'package:json_annotation/json_annotation.dart';

part 'aPhWr.g.dart';

@JsonSerializable()
class APhWr {
  final int? id;
  final String? t;
  final String? tfName;
  final int? sd;

  @JsonKey(ignore: true)
  PlayListItem? playListItem; // for sPlayer

  APhWr({this.id, this.t, this.tfName, this.sd});

  factory APhWr.fromJson(Map<String, dynamic> data) => _$APhWrFromJson(data);

  Map<String, dynamic> toJson() => _$APhWrToJson(this);
}
