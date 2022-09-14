import 'package:flutter/material.dart';

class ColorsUtils {
  static Map<int, Color> colorYellowCodes = {
    50: const Color.fromRGBO(255, 188, 59, .1),
    100: const Color.fromRGBO(255, 188, 59, .2),
    200: const Color.fromRGBO(255, 188, 59, .3),
    300: const Color.fromRGBO(255, 188, 59, .4),
    400: const Color.fromRGBO(255, 188, 59, .5),
    500: const Color.fromRGBO(255, 188, 59, .6),
    600: const Color.fromRGBO(255, 188, 59, .7),
    700: const Color.fromRGBO(255, 188, 59, .8),
    800: const Color.fromRGBO(255, 188, 59, .9),
    900: const Color.fromRGBO(255, 188, 59, 1),
  };

  static MaterialColor customYellow =
      MaterialColor(0xFFffbc3b, colorYellowCodes);

  static Map<int, Color> colorBlackCodes = {
    50: const Color.fromRGBO(72, 72, 95, .1),
    100: const Color.fromRGBO(72, 72, 95, .2),
    200: const Color.fromRGBO(72, 72, 95, .3),
    300: const Color.fromRGBO(72, 72, 95, .4),
    400: const Color.fromRGBO(72, 72, 95, .5),
    500: const Color.fromRGBO(72, 72, 95, .6),
    600: const Color.fromRGBO(72, 72, 95, .7),
    700: const Color.fromRGBO(72, 72, 95, .8),
    800: const Color.fromRGBO(72, 72, 95, .9),
    900: const Color.fromRGBO(72, 72, 95, 1),
  };
  static MaterialColor customBlack = MaterialColor(0xFF48485F, colorBlackCodes);

  static Color customBlackColor = const Color.fromRGBO(72, 72, 95, 1);
  static Color customYellowColor = const Color.fromRGBO(255, 188, 59, 1);
}
