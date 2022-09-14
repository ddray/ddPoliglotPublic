// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'list_arg.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ListArg _$ListArgFromJson(Map<String, dynamic> json) => ListArg(
      json['sort'] as String,
      json['order'] as String,
      json['page'] as int,
      json['pageSize'] as int,
    );

Map<String, dynamic> _$ListArgToJson(ListArg instance) => <String, dynamic>{
      'sort': instance.sort,
      'order': instance.order,
      'page': instance.page,
      'pageSize': instance.pageSize,
    };
