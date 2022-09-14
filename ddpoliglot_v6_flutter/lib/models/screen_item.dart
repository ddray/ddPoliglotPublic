import 'package:ddpoliglot_v6_flutter/models/play_list_item.dart';
import 'package:flutter/material.dart';

import '../models/player_classes.dart';
import '../models/screen_section.dart';
import './data/Word.dart';
import './data/WordPhrase.dart';

class ScreenItem {
  String videoCurrentText = '';
  String videoCurrentPronText = '';
  String videoCurrentTranslationText = '';
  var caseType = CurrentScreenTypes.dictorTextFirst.index;
  late ScreenSection section1;
  late ScreenSection section2;
  late ScreenSection section3;

  ScreenItem.fromPlayListItem(PlayListItem playListItem) {
    section1 = ScreenSection(3, Alignment.bottomCenter, 45, Colors.green, true);
    section2 =
        ScreenSection(2, Alignment.topCenter, 35, Colors.grey.shade300, true);
    section3 =
        ScreenSection(5, Alignment.topCenter, 1, Colors.grey.shade100, true);

    prepareTextsAndTypes(playListItem);
  }

  void prepareTextsAndTypes(PlayListItem playListItem) {
    Color textColor1 = Colors.grey.shade800;
    Color translationColor1 = Colors.grey.shade600;
    Color pronColor1 = Colors.grey.shade400;

    double textFontSize1 = 35;
    double translationFontSize1 = 35;
    double pronFontSize1 = 35;

    int textFlex1 = 3;
    int translationFlex1 = 3;
    int pronFlex1 = 4;

    int textFlex2 = 3;
    int translationFlex2 = 3;
    int pronFlex2 = 2;

    int phraseFlex1 = 7;
    int trPhraseFlex1 = 3;

    if (playListItem.itemType == PlayListItemTypes.word.index ||
        playListItem.itemType == PlayListItemTypes.wordSpeed1.index ||
        playListItem.itemType == PlayListItemTypes.wordSpeed2.index) {
      if (playListItem.activityType == PlayListActivityTypes.textFirst.index) {
        // TextYesPronYesTranslNoTextFirst = 0,
        caseType = CurrentScreenTypes.textYesPronYesTranslNoTextFirst.index;
        videoCurrentText = playListItem.text;
        videoCurrentPronText = playListItem.pronunciation;
        videoCurrentTranslationText = '';

        section1 = ScreenSection(textFlex1, Alignment.bottomCenter,
            getWordFontSize(videoCurrentText.length), textColor1, true);
        section2 = ScreenSection(
            pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, true);
        section3 = ScreenSection(translationFlex1, Alignment.center,
            translationFontSize1, translationColor1, true);
      } else {
        // TextYesPronYesTranslYesTranslFirst = 1,
        caseType = CurrentScreenTypes.textYesPronYesTranslYesTranslFirst.index;
        videoCurrentText = playListItem.text;
        videoCurrentPronText = playListItem.pronunciation;
        videoCurrentTranslationText = (playListItem.itemRef is Word)
            ? (playListItem.itemRef as Word).wordTranslation?.text ?? ''
            : (playListItem.itemRef as Map<String, dynamic>)['wordTranslation']
                    ['text'] ??
                '';

        section1 = ScreenSection(translationFlex2, Alignment.center,
            translationFontSize1, translationColor1, true);
        section2 = ScreenSection(
            textFlex2, Alignment.bottomCenter, textFontSize1, textColor1, true);
        section3 = ScreenSection(
            pronFlex2, Alignment.center, pronFontSize1, textColor1, true);
      }
    } else if (playListItem.itemType ==
        PlayListItemTypes.wordTranslation.index) {
      if (playListItem.activityType == PlayListActivityTypes.textFirst.index) {
        // TextYesPronYesTranslYesTextFirst = 2,
        caseType = CurrentScreenTypes.textYesPronYesTranslYesTextFirst.index;
        videoCurrentText = (playListItem.itemParentRef is Word)
            ? (playListItem.itemParentRef as Word).text
            : (playListItem.itemParentRef as Map<String, dynamic>)['text'];
        videoCurrentPronText = (playListItem.itemParentRef is Word)
            ? (playListItem.itemParentRef as Word).pronunciation ?? ''
            : (playListItem.itemParentRef
                    as Map<String, dynamic>)['pronunciation'] ??
                '';
        videoCurrentTranslationText = playListItem.text;

        section1 = ScreenSection(textFlex1, Alignment.bottomCenter,
            getWordFontSize(videoCurrentText.length), textColor1, true);
        section2 = ScreenSection(
            pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, true);
        section3 = ScreenSection(translationFlex1, Alignment.topCenter,
            translationFontSize1, translationColor1, true);
      } else {
        // TextNoPronNoTranslYesTranslFirst = 3,
        caseType = CurrentScreenTypes.textNoPronNoTranslYesTranslFirst.index;
        videoCurrentText = '';
        videoCurrentTranslationText = playListItem.text;

        section1 = ScreenSection(translationFlex2, Alignment.center,
            translationFontSize1, translationColor1, true);
        section2 = ScreenSection(
            textFlex2, Alignment.center, textFontSize1, textColor1, true);
        section3 = ScreenSection(
            pronFlex2, Alignment.center, pronFontSize1, textColor1, true);
      }
    } else if (playListItem.itemType == PlayListItemTypes.wordPhrase.index ||
        playListItem.itemType == PlayListItemTypes.wordPhraseSpeed1.index ||
        playListItem.itemType == PlayListItemTypes.wordPhraseSpeed2.index) {
      if (playListItem.activityType == PlayListActivityTypes.textFirst.index) {
        // TextYesTranslNoTextFirst = 4,
        caseType = CurrentScreenTypes.textYesTranslNoTextFirst.index;
        videoCurrentText = playListItem.text;
        videoCurrentTranslationText = '';

        section1 = ScreenSection(phraseFlex1, Alignment.center,
            getPhraseFontSize(videoCurrentText.length), textColor1, true);
        section2 = ScreenSection(
            pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, false);
        section3 = ScreenSection(
            trPhraseFlex1,
            Alignment.center,
            getTrPhraseFontSize(videoCurrentTranslationText.length),
            translationColor1,
            true);
      } else {
        // TextYesTranslYesTranslFirst = 5,
        caseType = CurrentScreenTypes.textYesTranslYesTranslFirst.index;
        videoCurrentText = playListItem.text;
        videoCurrentPronText = '';
        videoCurrentTranslationText = (playListItem.itemRef is WordPhrase)
            ? (playListItem.itemRef as WordPhrase)
                    .wordPhraseTranslation
                    ?.text ??
                ''
            : (playListItem.itemRef
                    as Map<String, dynamic>)['wordPhraseTranslation']['text'] ??
                '';
        section1 = ScreenSection(
            trPhraseFlex1,
            Alignment.center,
            getTrPhraseFontSize(videoCurrentTranslationText.length),
            translationColor1,
            true);
        section3 = ScreenSection(
            pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, false);
        section2 = ScreenSection(phraseFlex1, Alignment.center,
            getPhraseFontSize(videoCurrentText.length), textColor1, true);
      }
    } else if (playListItem.itemType ==
        PlayListItemTypes.wordPhraseTranslation.index) {
      if (playListItem.activityType == PlayListActivityTypes.textFirst.index) {
        // TextYesTranslYesTextFirst = 6,
        caseType = CurrentScreenTypes.textYesTranslYesTextFirst.index;
        videoCurrentText = (playListItem.itemParentRef is WordPhrase)
            ? (playListItem.itemParentRef as WordPhrase).text
            : (playListItem.itemParentRef as Map<String, dynamic>)['text'];
        videoCurrentTranslationText = playListItem.text;

        section1 = ScreenSection(phraseFlex1, Alignment.center,
            getPhraseFontSize(videoCurrentText.length), textColor1, true);
        section2 = ScreenSection(
            pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, false);
        section3 = ScreenSection(
            trPhraseFlex1,
            Alignment.center,
            getTrPhraseFontSize(videoCurrentTranslationText.length),
            translationColor1,
            true);
      } else {
        // TextNoTranslYesTranslFirst = 7,
        caseType = CurrentScreenTypes.textNoTranslYesTranslFirst.index;
        videoCurrentText = '';
        videoCurrentPronText = "";
        videoCurrentTranslationText = playListItem.text;
        section1 = ScreenSection(
            trPhraseFlex1,
            Alignment.center,
            getTrPhraseFontSize(videoCurrentTranslationText.length),
            translationColor1,
            true);
        section3 = ScreenSection(
            pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, false);
        section2 = ScreenSection(phraseFlex1, Alignment.center,
            getPhraseFontSize(videoCurrentText.length), textColor1, true);
      }
    } else if (playListItem.itemType == PlayListItemTypes.dictorPhrase.index) {
      // DictorTextFirst = 8,
      caseType = CurrentScreenTypes.dictorTextFirst.index;
      videoCurrentText = playListItem.text;
      videoCurrentTranslationText = '!!!';

      section1 = ScreenSection(phraseFlex1, Alignment.center,
          getPhraseFontSize(videoCurrentText.length), textColor1, true);
      section2 = ScreenSection(
          pronFlex1, Alignment.topCenter, pronFontSize1, pronColor1, false);
      section3 = ScreenSection(
          trPhraseFlex1,
          Alignment.center,
          getTrPhraseFontSize(videoCurrentTranslationText.length),
          translationColor1,
          true);
    }
  }

  double getWordFontSize(int length) {
    return length < 10
        ? 45
        : length < 15
            ? 35
            : 28;
  }

  double getPhraseFontSize(int length) {
    if (length < 50) {
      return 35;
    }
    if (length < 100) {
      return 25;
    }

    return 20;
  }

  double getTrPhraseFontSize(int length) {
    if (length < 50) {
      return 25;
    }
    if (length < 100) {
      return 18;
    }

    return 15;
  }
}
