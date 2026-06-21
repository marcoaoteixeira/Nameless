import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ChoresService } from '../../../../core/services/chores.service';

@Component({
  selector: 'app-create-chore-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './create-chore-modal.component.html'
})
export class CreateChoreModalComponent {
  private readonly fb = inject(FormBuilder);
  private readonly choresService = inject(ChoresService);

  created = output<void>();
  cancelled = output<void>();

  submitting = signal(false);
  error = signal<string | null>(null);

  form = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    dueDate: [null as string | null]
  });

  submit() {
    if (this.form.invalid || this.submitting()) return;
    this.submitting.set(true);
    this.error.set(null);

    const { title, description, dueDate } = this.form.getRawValue();
    this.choresService.create({
      title: title!,
      description: description!,
      dueDate: dueDate ? new Date(dueDate).toISOString() : null
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.created.emit();
      },
      error: () => {
        this.submitting.set(false);
        this.error.set('Failed to create chore. Please try again.');
      }
    });
  }
}
