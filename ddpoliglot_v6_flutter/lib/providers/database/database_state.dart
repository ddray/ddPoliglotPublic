import 'package:flutter/foundation.dart';
import 'package:sqflite/sqflite.dart';

import '../../models/article_by_param_data.dart';
import '../../models/data/user_lesson.dart';
import '../../models/dict_word.dart';

class DatabaseState {
  final Future<Database>? database;
  final List<DictWord>? dictWords;
  final List<UserLesson>? userLessons;
  final List<ArticleByParamData>? schemas;
  DatabaseState({
    required this.database,
    this.dictWords,
    this.userLessons,
    this.schemas,
  });

  DatabaseState copyWith({
    Future<Database>? database,
    List<DictWord>? dictWords,
    List<UserLesson>? userLessons,
    List<ArticleByParamData>? schemas,
  }) {
    return DatabaseState(
      database: database ?? this.database,
      dictWords: dictWords ?? this.dictWords,
      userLessons: userLessons ?? this.userLessons,
      schemas: schemas ?? this.schemas,
    );
  }

  factory DatabaseState.initial() {
    return DatabaseState(
      database: null,
      dictWords: null,
      userLessons: null,
      schemas: null,
    );
  }

  @override
  String toString() {
    return 'DatabaseState(database: $database, dictWords: $dictWords, userLessons: $userLessons, schemas: $schemas)';
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;

    return other is DatabaseState &&
        other.database == database &&
        listEquals(other.dictWords, dictWords) &&
        listEquals(other.userLessons, userLessons) &&
        listEquals(other.schemas, schemas);
  }

  @override
  int get hashCode {
    return database.hashCode ^
        dictWords.hashCode ^
        userLessons.hashCode ^
        schemas.hashCode;
  }
}
