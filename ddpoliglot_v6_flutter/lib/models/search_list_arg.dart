import 'package:json_annotation/json_annotation.dart';

part 'search_list_arg.g.dart';

@JsonSerializable()
class SearchListArg {
  String sort = "";
  String order = "";
  int page = 0;
  int pageSize = 0;
  int parentID = 0;
  String searchText = "";
  int rateFrom = 0;
  int rateTo = 0;
  int gradeFrom = 0;
  int gradeTo = 0;
  bool recomended = false;
  String str1 = "";
  String str2 = "";
  int int1 = 0;
  int int2 = 0;

  SearchListArg.empty();

  SearchListArg(
      this.sort,
      this.order,
      this.page,
      this.pageSize,
      this.parentID,
      this.searchText,
      this.rateFrom,
      this.rateTo,
      this.gradeFrom,
      this.gradeTo,
      this.recomended,
      this.str1,
      this.str2,
      this.int1,
      this.int2);

  factory SearchListArg.fromJson(Map<String, dynamic> data) =>
      _$SearchListArgFromJson(data);

  Map<String, dynamic> toJson() => _$SearchListArgToJson(this);
}
