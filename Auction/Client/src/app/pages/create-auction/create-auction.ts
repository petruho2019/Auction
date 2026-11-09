import { HttpErrorResponse } from "@angular/common/http";
import { Component, inject, signal } from "@angular/core";
import {
  AbstractControl,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from "@angular/forms";
import { AuctionResponse } from "../../models/auction-response";
import { AuctionService } from "../../services/auction.service";

const maxPhotoSize = 10 * 1024 * 1024;
const allowedPhotoExtensions = new Set([".jpg", ".jpeg", ".png", ".webp"]);

const nonWhitespace: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null =>
  typeof control.value === "string" && control.value.trim().length > 0
    ? null
    : { required: true };

const validPhoto: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const file: unknown = control.value;
  if (file === null || file === undefined) {
    return null;
  }

  if (!(file instanceof File)) {
    return { invalidFile: true };
  }

  const extension = file.name.slice(file.name.lastIndexOf(".")).toLowerCase();
  if (!allowedPhotoExtensions.has(extension)) {
    return { invalidExtension: true };
  }

  return file.size > maxPhotoSize ? { maxSize: true } : null;
};

const endsAfterStarts: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const startsAt: unknown = control.get("startsAt")?.value;
  const endsAt: unknown = control.get("endsAt")?.value;
  if (
    typeof startsAt !== "string" ||
    typeof endsAt !== "string" ||
    !startsAt ||
    !endsAt
  ) {
    return null;
  }

  const startTime = new Date(startsAt).getTime();
  const endTime = new Date(endsAt).getTime();
  return Number.isNaN(startTime) || Number.isNaN(endTime) || endTime > startTime
    ? null
    : { invalidDateRange: true };
};

@Component({
  imports: [ReactiveFormsModule, FormsModule],
  selector: "app-create-auction",
  styleUrl: "./create-auction.scss",
  templateUrl: "./create-auction.html",
})
export class CreateAuction {
  private readonly formBuilder = inject(FormBuilder).nonNullable;
  private readonly auctionService = inject(AuctionService);

  protected readonly isSubmitting = signal(false);
  protected readonly submitError = signal<string | null>(null);
  protected readonly createdAuction = signal<AuctionResponse | null>(null);
  protected readonly form = this.formBuilder.group(
    {
      productName: [
        "",
        [Validators.required, nonWhitespace, Validators.maxLength(200)],
      ],
      startsAt: ["", Validators.required],
      endsAt: ["", Validators.required],
      photo: this.formBuilder.control<File | null>(null, validPhoto),
    },
    { validators: endsAfterStarts },
  );

  protected onPhotoSelected(event: Event): void {
    const input = event.target;
    if (!(input instanceof HTMLInputElement)) {
      return;
    }

    this.form.controls.photo.setValue(input.files?.item(0) ?? null);
    this.form.controls.photo.markAsTouched();
    this.createdAuction.set(null);
    this.submitError.set(null);
  }

  protected submit(photoInput: HTMLInputElement): void {
    if (this.form.invalid || this.isSubmitting()) {
      this.form.markAllAsTouched();
      return;
    }

    const { productName, startsAt, endsAt, photo } = this.form.getRawValue();
    const formData = new FormData();
    formData.append("ProductName", productName.trim());
    formData.append("StartsAt", new Date(startsAt).toISOString());
    formData.append("EndsAt", new Date(endsAt).toISOString());
    if (photo) {
      formData.append("Photo", photo);
    }

    this.isSubmitting.set(true);
    this.submitError.set(null);
    this.createdAuction.set(null);
    this.auctionService.create(formData).subscribe({
      next: (auction) => {
        this.createdAuction.set(auction);
        this.form.reset();
        photoInput.value = "";
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.submitError.set(this.getErrorMessage(error));
        this.isSubmitting.set(false);
      },
    });
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse && error.status === 0) {
      return "Не удалось связаться с сервером. Проверьте, что Auction API запущен.";
    }

    if (error instanceof HttpErrorResponse && error.status === 400) {
      return "Проверьте введённые данные и формат фотографии.";
    }

    return "Не удалось создать аукцион. Попробуйте ещё раз.";
  }
}
