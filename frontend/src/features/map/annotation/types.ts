export interface PendingAnnotation {
  wkt: string;
  x:   number;   
  y:   number;
}

export interface AnnotationResponseDto {
  id:          number;
  noteText:    string;
  wktGeometry: string;
  userId:      number;
  teamId:      number | null;
  createdDate: string;
}