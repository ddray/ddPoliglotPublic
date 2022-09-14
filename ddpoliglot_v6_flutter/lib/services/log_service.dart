import 'dart:convert';

import '../models/data/Log.dart';
import '../utils/http_utils.dart';

class LogService {
  static Future<void> add(Log log) async {
    await HttpUtils.post('Log/Add', jsonEncode(log));
    return;
  }
}
