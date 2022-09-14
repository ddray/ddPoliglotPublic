// ignore_for_file: prefer_interpolation_to_compose_strings, prefer_adjacent_string_concatenation

import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:path/path.dart';
import 'package:sqflite/sqflite.dart';

class DatabaseService {
  static Future<Database> openDb(String databaseName) async {
    return openDatabase(join(await getDatabasesPath(), databaseName),
        onCreate: (db, version) {
      // dictWords
      db.execute('CREATE TABLE dictWords(' +
          'wordID INTEGER PRIMARY KEY,' +
          ' text TEXT,' +
          ' rate INTEGER,' +
          ' pronunciation TEXT,' +
          ' translation TEXT,' +
          ' oxfordLevel INTEGER,' +
          ' wordUserID INTEGER,' +
          ' grade INTEGER,' +
          ' lastRepeatInArticleByParamID INTEGER,' +
          ' lastRepeatInLessonNum INTEGER,' +
          ' lastRepeatWordPhraseId INTEGER,' +
          ' sourceType INTEGER,' +
          ' textClear TEXT,' +
          ' readyForLesson INTEGER' +
          ' )');
      // vversions
      db.execute('CREATE TABLE vversions(' +
          'id INTEGER PRIMARY KEY,' +
          ' value INTEGER' +
          ' )');
      // vversions
      db.execute('CREATE TABLE schema_versions(' +
          'id INTEGER PRIMARY KEY,' +
          ' value INTEGER' +
          ' )');
      // schema of lesson
      db.execute('CREATE TABLE schemaLessonGen(' +
          ' lessonType INTEGER,' +
          ' jsonData TEXT' +
          ' )');
      // userLessons
      db.execute('CREATE TABLE userLessons(' +
          ' userLessonID INTEGER PRIMARY KEY,' +
          ' languageID INTEGER,' +
          ' userID TEXT,' +
          ' num INTEGER,' +
          ' created TEXT,' +
          ' updated TEXT,' +
          ' finished TEXT,' +
          ' totalSeconds INTEGER,' +
          ' learnSeconds INTEGER,' +
          ' lessonType INTEGER,' +
          ' userLessonWordsJson TEXT,' +
          ' jsonData TEXT' +
          ' )');
      db.insert(
        'vversions',
        {'id': 1, 'value': 0},
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
      db.insert(
        'schema_versions',
        {'id': 1, 'value': 0},
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }, onUpgrade: (db, oldVersion, newVersion) {}, version: 1);
  }

  static Future<void> dictWordUpdate(
      Future<Database>? database, DictWord dictWord) async {
    final db = await database;
    await db!.update('dictWords', dictWord.toJson(),
        where: 'wordID=${dictWord.wordID}');
  }

  static Future<void> userLessonInsert(Future<Database>? database,
      UserLesson userLesson, List<UserLesson>? userLessons) async {
    final db = await database;
    await db!.insert('userLessons', userLesson.toDbJson());
    if (userLessons != null) {
      userLessons.add(userLesson);
    }
  }

  static Future<void> userLessonUpdate(Future<Database>? database,
      UserLesson userLesson, List<UserLesson>? userLessons) async {
    final db = await database;
    await db!.update('userLessons', userLesson.toDbJson(),
        where: 'userLessonID = ?', whereArgs: [userLesson.userLessonID]);

    if (userLessons != null) {
      var ul = userLessons.firstWhere((x) => x.num == userLesson.num);
      ul.copyFrom(userLesson);
    }
  }

  static Future<UserLesson?> userLessonById(
      Future<Database>? database, int lessonNum) async {
    final db = await database;
    var result = await db!
        .query('userLessons', where: 'num = ?', whereArgs: [lessonNum]);
    return result.length == 1 ? UserLesson.fromDbJson(result[0]) : null;
  }
}
