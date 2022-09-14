import 'package:ddpoliglot_v6_flutter/widgets/quest_question_widget.dart';
import 'package:flutter/material.dart';

class QuestScreenWidget extends StatelessWidget {
  const QuestScreenWidget({Key? key, this.question, this.answers})
      : super(key: key);
  final String? question;
  final List<Widget>? answers;
  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Expanded(flex: 1, child: QuestQuestionWidget(question ?? '')),
        Expanded(
            flex: 4,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 10),
              alignment: Alignment.center,
              child: ListView(
                children: answers ?? [],
              ),
            )),
      ],
    );
  }
}
