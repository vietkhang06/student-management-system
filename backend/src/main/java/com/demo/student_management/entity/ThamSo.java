package com.demo.student_management.entity;

import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "THAMSO")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class ThamSo {

    @Id
    @Column(name = "ID_THAMSO", length = 5, nullable = false)
    @EqualsAndHashCode.Include
    private String idThamSo;

    @Column(name = "TEN_THAMSO", length = 255)
    private String tenThamSo;

    @Column(name = "KIEU_THAMSO")
    private Integer kieuThamSo;

    @Column(name = "GIATRI_THAMSO", length = 255)
    private String giaTriThamSo;
}