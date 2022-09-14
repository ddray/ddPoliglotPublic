// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'search_list_arg.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SearchListArg _$SearchListArgFromJson(Map<String, dynamic> json) =>
    SearchListArg(
      json['sort'] as String,
      json['order'] as String,
      json['page'] as int,
      json['pageSize'] as int,
      json['parentID'] as int,
      json['searchText'] as String,
      json['rateFrom'] as int,
      json['rateTo'] as int,
      json['gradeFrom'] as int,
      json['gradeTo'] as int,
      json['recomended'] as bool,
      json['str1'] as String,
      json['str2'] as String,
      json['int1'] as int,
      json['int2'] as int,
    );

Map<String, dynamic> _$SearchListArgToJson(SearchListArg instance) =>
    <String, dynamic>{
      'sort': instance.sort,
      'order': instance.order,
      'page': instance.page,
      'pageSize': instance.pageSize,
      'parentID': instance.parentID,
      'searchText': instance.searchText,
      'rateFrom': instance.rateFrom,
      'rateTo': instance.rateTo,
      'gradeFrom': instance.gradeFrom,
      'gradeTo': instance.gradeTo,
      'recomended': instance.recomended,
      'str1': instance.str1,
      'str2': instance.str2,
      'int1': instance.int1,
      'int2': instance.int2,
    };
