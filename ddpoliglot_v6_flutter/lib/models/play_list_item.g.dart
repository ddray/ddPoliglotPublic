// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'play_list_item.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlayListItem _$PlayListItemFromJson(Map<String, dynamic> json) => PlayListItem(
      itemID: json['itemID'] as int? ?? 0,
      itemParentID: json['itemParentID'] as int? ?? 0,
      itemRef: json['itemRef'],
      itemParentRef: json['itemParentRef'],
      text: json['text'] as String? ?? "",
      pronunciation: json['pronunciation'] as String? ?? "",
      textSpeechFileName: json['textSpeechFileName'] as String? ?? "",
      speachDuration: json['speachDuration'] as int? ?? 0,
      pause: json['pause'] as int? ?? 0,
      itemType: json['itemType'] as int? ?? -1,
      activityType: json['activityType'] as int? ?? 0,
      order: json['order'] as int?,
      isCurrent: json['isCurrent'] as bool?,
      fromSecond: json['fromSecond'] as int?,
      childrenType: json['childrenType'] as int? ?? 0,
      srcWordsQty: json['srcWordsQty'] as int? ?? -1,
    );

Map<String, dynamic> _$PlayListItemToJson(PlayListItem instance) =>
    <String, dynamic>{
      'itemID': instance.itemID,
      'itemParentID': instance.itemParentID,
      'itemRef': instance.itemRef,
      'itemParentRef': instance.itemParentRef,
      'text': instance.text,
      'pronunciation': instance.pronunciation,
      'textSpeechFileName': instance.textSpeechFileName,
      'speachDuration': instance.speachDuration,
      'pause': instance.pause,
      'itemType': instance.itemType,
      'activityType': instance.activityType,
      'order': instance.order,
      'isCurrent': instance.isCurrent,
      'fromSecond': instance.fromSecond,
      'childrenType': instance.childrenType,
      'srcWordsQty': instance.srcWordsQty,
    };
