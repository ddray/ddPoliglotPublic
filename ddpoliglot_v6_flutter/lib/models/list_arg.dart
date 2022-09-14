import 'package:json_annotation/json_annotation.dart';

part 'list_arg.g.dart';

@JsonSerializable()
class ListArg {
  final String sort;
  final String order;
  final int page;
  final int pageSize;

  ListArg(
    this.sort,
    this.order,
    this.page,
    this.pageSize,
  );

  factory ListArg.fromJson(Map<String, dynamic> data) =>
      _$ListArgFromJson(data);

  Map<String, dynamic> toJson() => _$ListArgToJson(this);
}
