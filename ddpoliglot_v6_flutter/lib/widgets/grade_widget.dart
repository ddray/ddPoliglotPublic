import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:flutter/material.dart';

enum WordItemState {
  neutral,
  unknown,
  knoun,
  inProcess,
}

class WordSelectRepeatItem extends StatefulWidget {
  const WordSelectRepeatItem({
    Key? key,
    this.onChangeItemState,
    this.isLast = false,
    required this.dictWord,
  }) : super(key: key);

  final Function(int state)? onChangeItemState;
  final DictWord dictWord;
  final bool isLast;
  @override
  State<WordSelectRepeatItem> createState() => _WordSelectRepeatItemState();
}

class _WordSelectRepeatItemState extends State<WordSelectRepeatItem> {
  @override
  Widget build(BuildContext context) {
    final word = widget.dictWord;
    final curState = (word.selected && word.grade > 0 && word.grade < 5)
        ? WordItemState.inProcess
        : WordItemState.knoun;

    return Container(
      margin: widget.isLast ? const EdgeInsets.only(bottom: 80) : null,
      padding: const EdgeInsets.symmetric(vertical: 6),
      decoration: BoxDecoration(
        border: Border(
          bottom: BorderSide(width: 1.0, color: Colors.grey.shade100),
        ),
        // color: Colors.white,
      ),
      child: Row(
        children: [
          RateWidget(
            rate: word.rate,
          ),
          WordTextWidget(
            text: word.text,
            translation: word.translation ?? '',
            selected: word.selected,
          ),
          widget.onChangeItemState != null
              ? WordState2ButtonWidget(
                  curState: curState,
                  onChangeItemState: (state) {
                    widget.onChangeItemState!(state);
                  },
                )
              : const SizedBox(
                  height: 40,
                )
        ],
      ),
    );
  }
}

class WordTextWidget extends StatelessWidget {
  const WordTextWidget({
    Key? key,
    required this.text,
    required this.translation,
    required this.selected,
  }) : super(key: key);

  final String text;
  final String translation;
  final bool selected;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            text,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(
                fontSize: 20,
                fontWeight: selected ? FontWeight.bold : FontWeight.normal),
          ),
          Text(
            translation,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(
                fontSize: 10,
                fontWeight: FontWeight.normal,
                color: Colors.grey[600]),
          ),
        ],
      ),
    );
  }
}

class RateWidget extends StatelessWidget {
  const RateWidget({
    Key? key,
    required this.rate,
  }) : super(key: key);

  final int rate;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: 40,
      height: 60,
      child: Container(
        alignment: Alignment.centerLeft,
        child: Text(
          rate.toString(),
          style: TextStyle(
              color: Colors.grey[400],
              fontSize: 10,
              fontWeight: FontWeight.bold),
        ),
      ),
    );
  }
}

class WordSelectStateItem extends StatefulWidget {
  const WordSelectStateItem({
    Key? key,
    this.onChangeItemState,
    this.isLast = false,
    required this.dictWord,
  }) : super(key: key);

  final Function(int state)? onChangeItemState;
  final DictWord dictWord;
  final bool isLast;

  @override
  State<WordSelectStateItem> createState() => _WordSelectStateItemState();
}

class _WordSelectStateItemState extends State<WordSelectStateItem> {
  @override
  Widget build(BuildContext context) {
    final word = widget.dictWord;
    final curState = (!word.selected && word.grade == 0)
        ? WordItemState.neutral
        : (word.selected && word.grade == 0)
            ? WordItemState.unknown
            : WordItemState.knoun;

    return Container(
      margin: widget.isLast ? const EdgeInsets.only(bottom: 80) : null,
      padding: const EdgeInsets.symmetric(vertical: 1),
      decoration: BoxDecoration(
        border: Border(
          bottom: BorderSide(width: 1.0, color: Colors.grey.shade100),
        ),
        // color: Colors.white,
      ),
      child: Row(
        children: [
          RateWidget(
            rate: widget.dictWord.rate,
          ),
          WordTextWidget(
            text: word.text,
            translation: word.translation ?? '',
            selected: word.selected,
          ),
          WordState3ButtonWidget(
              curState: curState, onChangeItemState: widget.onChangeItemState),
        ],
      ),
    );
  }
}

class WordState2ButtonWidget extends StatelessWidget {
  const WordState2ButtonWidget(
      {Key? key, required this.curState, this.onChangeItemState})
      : super(key: key);
  final WordItemState curState;
  final Function(int state)? onChangeItemState;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        IconButton(
          splashColor: ColorsUtils.customYellowColor,
          icon: Icon(
            Icons.sentiment_satisfied_alt,
            color: curState == WordItemState.knoun
                ? Colors.amber[800]
                : Colors.grey[300],
          ),
          onPressed: () {
            if (onChangeItemState != null) {
              final newState = curState == WordItemState.knoun
                  ? WordItemState.inProcess
                  : WordItemState.knoun;
              onChangeItemState!(newState.index);
            }
          },
        ),
        IconButton(
          splashColor: ColorsUtils.customYellowColor,
          icon: Stack(
            children: [
              Padding(
                padding: curState == WordItemState.inProcess
                    ? const EdgeInsets.fromLTRB(2, 2, 0, 0)
                    : const EdgeInsets.all(0),
                child: Icon(
                  Icons.school,
                  color: curState == WordItemState.knoun
                      ? Colors.grey[300]
                      : ColorsUtils.customYellowColor,
                  size: curState == WordItemState.inProcess ? 20 : null,
                ),
              ),
              if (curState == WordItemState.inProcess)
                Padding(
                  padding: const EdgeInsets.fromLTRB(8, 10, 0, 0),
                  child: Icon(
                    Icons.check,
                    size: 25,
                    color: ColorsUtils.customBlackColor,
                  ),
                )
            ],
          ),
          onPressed: () {
            if (onChangeItemState != null) {
              final newState = curState == WordItemState.inProcess
                  ? WordItemState.knoun
                  : WordItemState.inProcess;
              onChangeItemState!(newState.index);
            }
          },
        ),
      ],
    );
  }
}

class WordState3ButtonWidget extends StatelessWidget {
  const WordState3ButtonWidget(
      {Key? key, required this.curState, this.onChangeItemState})
      : super(key: key);
  final WordItemState curState;
  final Function(int state)? onChangeItemState;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        IconButton(
          splashColor: ColorsUtils.customYellowColor,
          icon: Icon(
            Icons.sentiment_satisfied_alt,
            color: curState == WordItemState.knoun
                ? Colors.amber[800]
                : curState == WordItemState.neutral
                    ? Colors.grey[400]
                    : Colors.grey[200],
          ),
          onPressed: () {
            if (onChangeItemState != null) {
              onChangeItemState!(curState == WordItemState.knoun
                  ? WordItemState.neutral.index
                  : WordItemState.knoun.index);
            }
          },
        ),
        IconButton(
          splashColor: ColorsUtils.customYellowColor,
          icon: Stack(
            children: [
              Padding(
                padding: curState == WordItemState.unknown
                    ? const EdgeInsets.fromLTRB(2, 2, 0, 0)
                    : const EdgeInsets.all(0),
                child: Icon(
                  Icons.school,
                  color: curState == WordItemState.knoun
                      ? Colors.grey[200]
                      : curState == WordItemState.neutral
                          ? Colors.grey[400]
                          : ColorsUtils.customYellowColor,
                  size: curState == WordItemState.knoun
                      ? null
                      : curState == WordItemState.neutral
                          ? null
                          : 20,
                ),
              ),
              if (curState == WordItemState.unknown)
                Padding(
                  padding: const EdgeInsets.fromLTRB(8, 10, 0, 0),
                  child: Icon(
                    Icons.check,
                    size: 25,
                    color: ColorsUtils.customBlackColor,
                  ),
                )
            ],
          ),
          onPressed: () {
            if (onChangeItemState != null) {
              onChangeItemState!(curState == WordItemState.unknown
                  ? WordItemState.neutral.index
                  : WordItemState.unknown.index);
            }
          },
        ),
      ],
    );
  }
}

class GradeConfirmationDialog extends StatefulWidget {
  const GradeConfirmationDialog({Key? key, required this.type})
      : super(key: key);
  final int type;
  @override
  GradeConfirmationDialogState createState() => GradeConfirmationDialogState();
}

class GradeConfirmationDialogState extends State<GradeConfirmationDialog> {
  bool checked = false;

  @override
  Widget build(BuildContext context) {
    return ConstrainedBox(
      constraints: const BoxConstraints(maxHeight: 350, minHeight: 200),
      child: SingleChildScrollView(
        child: Column(
          children: [
            Container(
              padding: const EdgeInsets.fromLTRB(10, 10, 10, 10),
              child: ListBody(
                children: <Widget>[
                  widget.type == 1
                      ? const Text('Вы помечаете слово как изученное.')
                      : const Text('Вы помечаете слово как не изученное.'),
                  const SizedBox(
                    height: 5,
                  ),
                  widget.type == 1
                      ? const Text(
                          'Теперь это слово не будет предлагаться для изучения.')
                      : const Text(
                          'Это слово становится кандидатом на изучение в уроках.'),
                  // widget.type == 1
                  //     ? const Text(
                  //         'Чем ниже рейтинг, тем чаще оно будет встречатся для повторения в следующих уроках.')
                  //     : const Text(''),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Checkbox(
                          value: checked,
                          onChanged: (v) {
                            setState(() {
                              checked = v ?? false;
                            });
                          }),
                      const Text('Не показывать опять.'),
                    ],
                  )
                ],
              ),
            ),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                TextButton(
                  child: const Text('Отмена'),
                  onPressed: () {
                    Navigator.pop(context, checked ? 10 : 0);
                  },
                ),
                TextButton(
                  child: const Text('Ok'),
                  onPressed: () {
                    Navigator.pop(context, checked ? 11 : 1);
                  },
                ),
              ],
            )
          ],
        ),
      ),
    );
  }
}
