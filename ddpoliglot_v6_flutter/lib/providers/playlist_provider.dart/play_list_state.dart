import '../../models/user_lesson_data.dart';

class PlayListState {
  final UserLessonData? userLessonData;
  final int? preparationStep;
  PlayListState({
    this.userLessonData,
    this.preparationStep,
  });

  PlayListState copyWith({
    UserLessonData? userLessonData,
    int? preparationStep,
  }) {
    return PlayListState(
      userLessonData: userLessonData ?? this.userLessonData,
      preparationStep: preparationStep ?? this.preparationStep,
    );
  }

  factory PlayListState.initial() {
    return PlayListState(userLessonData: null, preparationStep: null);
  }

  bool get isEmpty => userLessonData == null && preparationStep == null;

  @override
  String toString() =>
      'PlayListState(userLessonData: $userLessonData, preparationStep: $preparationStep)';

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;

    return other is PlayListState &&
        other.userLessonData == userLessonData &&
        other.preparationStep == preparationStep;
  }

  @override
  int get hashCode => userLessonData.hashCode ^ preparationStep.hashCode;
}
