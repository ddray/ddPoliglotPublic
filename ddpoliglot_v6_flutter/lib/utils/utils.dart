import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class Utils {
  static void showErrorDialog(String message, BuildContext context) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Ошибка!'),
        content: Text(message),
        actions: <Widget>[
          TextButton(
            child: const Text('Ok'),
            onPressed: () {
              Navigator.of(ctx).pop();
            },
          )
        ],
      ),
    );
  }

  static Future<dynamic> showInputDialog(
      String message, BuildContext context) async {
    TextEditingController textFieldController = TextEditingController();
    return showDialog(
        context: context,
        builder: (ctx) => AlertDialog(
              title: const Text('AlertDemo with TextField '),
              content: TextField(
                controller: textFieldController,
                decoration: const InputDecoration(hintText: "Enter Text"),
              ),
              actions: [
                TextButton(
                  child: const Text('SUBMIT'),
                  onPressed: () {
                    Navigator.pop(context, textFieldController.text);
                  },
                )
              ],
            ));
  }
}
