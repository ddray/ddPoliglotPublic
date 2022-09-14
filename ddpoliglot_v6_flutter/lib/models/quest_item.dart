class QuestItem {
  final int level;
  final String text;
  final List<QuestAnswer> answers;
  QuestItem(this.level, this.text, this.answers);
}

class QuestAnswer {
  final String text;
  final bool correct;
  QuestAnswer(this.text, this.correct);
}
