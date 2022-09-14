import 'package:ddpoliglot_v6_flutter/models/play_list_item.dart';
// ignore_for_file: file_names
import 'package:json_annotation/json_annotation.dart';

part 'play_list.g.dart';

@JsonSerializable()
class PlayList {
  late String guid;
  late String name;
  late bool active;
  late bool? isCurrent;
  late List<PlayListItem> items;
  late int totalSeconds;
  late int learnSeconds;

  PlayList(this.guid, this.name, this.active, this.isCurrent, this.items,
      this.totalSeconds, this.learnSeconds) {
    //recalculate();
  }

  // recalculate() {
  //   var second = 0;
  //   for (var i = 0; i < items.length; i++) {
  //     items[i].order = i;
  //     items[i].fromSecond = second;
  //     second += (items[i].speachDuration + items[i].pause);
  //   }
  // }

  PlayListItem getByIndex(index) {
    return items[index];
  }

  length() {
    return items.length;
  }

  PlayListItem getBySeconds(seconds) {
    var arr = List.from(items.reversed);
    return arr.first((x) => x.fromSecond <= seconds);
  }

  List<PlayListItem> getRest(int audioCurrentIndex) {
    List<PlayListItem> list = [];
    for (var i = 0; i < items.length; i++) {
      if (i >= audioCurrentIndex) {
        list.add(items[i]);
      }
    }

    return list;
  }

  getItems() {
    return items;
  }

  setCurrent(int index) {
    for (var item in items) {
      item.isCurrent = false;
    }

    getByIndex(index).isCurrent = true;
  }

  int getDurationInSeconds() {
    var res = 0;
    for (var item in items) {
      res += (item.speachDuration + item.pause);
    }

    return res;
  }

  factory PlayList.fromJson(Map<String, dynamic> data) =>
      _$PlayListFromJson(data);

  Map<String, dynamic> toJson() => _$PlayListToJson(this);
}
